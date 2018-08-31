using Refit;
using System.Net.Http;
using System;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.Core.ApiDefinitions;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Polly;
using Plugin.Connectivity;
using XDemo.Core.Infrastructure.Logging;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace XDemo.Core.Infrastructure.Networking
{
    public static class RestServiceHelper
    {
        public static TApi GetApi<TApi>()
        {
            // set default refit setting
            var defaultSettings = new RefitSettings
            {
                JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                }
            };
            var client = GetHttpClient();
            return RestService.For<TApi>(client, defaultSettings);
        }

        public static async Task<T> CallWithRetry<T>(Task<T> apiTask, RetryMode retryMode = RetryMode.Confirm) where T : DtoBase, new()
        {
            var result = new T();
            switch (retryMode)
            {
                case RetryMode.None:
                    try
                    {
                        result = await apiTask;
                    }
                    catch (TaskCanceledException ex)
                    {
                        result.Result = ApiResult.Canceled;
                        result.ErrorMessage = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        result.Result = ApiResult.Failed;
                        result.ErrorMessage = ex.Message;
                    }
                    break;
                case RetryMode.Warning:
                    var warningRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        await WarningOnMainThread();
                    });
                    result = await warningRetryPolicy.ExecuteAsync(() => ActionSendAsync(apiTask));
                    break;
                case RetryMode.Confirm:
                    var confirmRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        var sure = await ConfirmOnMainThread();
                        if (!sure)
                        {
                            var orgTcs = context["tokenSource"] as CancellationTokenSource;
                            orgTcs.Cancel();
                        }
                    });
                    var inputTcs = new CancellationTokenSource();
                    try
                    {
                        //passed the token into it to cancel if the user dont choose retry
                        result = await confirmRetryPolicy.ExecuteAsync((inputContext, token) => ActionSendAsync(apiTask), new Context { { "tokenSource", inputTcs } }, inputTcs.Token).ConfigureAwait(true);
                    }
                    catch (OperationCanceledException ex)
                    {
                        //ignore: the retry circle broken
                        LogCommon.Error(ex);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(retryMode), $"The retry mode '{retryMode}' is not supported yet!");
            }

            //finish up
            return result;
        }

        #region private methods, executor
        static HttpClient GetHttpClient()
        {
            var src = new CancellationTokenSource();
            var handler = new ExtendedNativeMessageHandler();
            var toReturn = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiHosts.MainHost),
                Timeout = TimeSpan.FromSeconds(10),
            };
            return toReturn;
        }

        static async Task<T> ActionSendAsync<T>(Task<T> task) where T : DtoBase, new()
        {
            try
            {
                //precondition by connectivity
                if (!CrossConnectivity.Current.IsConnected)
                    throw new WebException("There's no internet connections!");
                //undone: if the api task failed, its still faulted forever, how can we renew it?
                return await task;
            }
            catch (OperationCanceledException)
            {
                //ignored: the api inner call canceled by user
                return default(T);
            }
            catch (AggregateException ex)
            {
                if (!(ex.InnerException is OperationCanceledException))
                    throw ex;
                return default(T);
            }
            catch (Exception ex)
            {
                //rethrown the exception
                throw ex;
            }
        }

        /// <summary>
        /// Confirms the on main thread.
        /// </summary>
        /// <returns>The on main thread.</returns>
        private static Task WarningOnMainThread()
        {
            // no async await.
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var result = Application.Current.MainPage.DisplayAlert("Warning", "Warning message!", "Ok");
                // Set result from MainThread to tcs.Task
                result.ContinueWith((sender) => tcs.SetResult(true));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Confirms the on main thread.
        /// </summary>
        /// <returns>The on main thread.</returns>
        private static Task<bool> ConfirmOnMainThread()
        {
            // no async await.
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var result = Application.Current.MainPage.DisplayAlert("confirm", "Confirm message?", "Ok", "Cancel");
                // Set result from MainThread to tcs.Task
                result.ContinueWith((sender) => tcs.SetResult(sender.Result));
            });
            return tcs.Task;
        }
        #endregion
    }
}