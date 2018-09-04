using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Polly;
using Refit;
using Xamarin.Forms;
using XDemo.Core.ApiDefinitions;
using XDemo.Core.Infrastructure.Logging;
using XDemo.Core.Infrastructure.Networking.Base;

namespace XDemo.Core.Infrastructure.Networking.Refit
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
            return RestService.For<TApi>(GetHttpClient(), defaultSettings);
        }

        /// <summary>
        /// Calls the api task with retry.
        /// </summary>
        /// <returns>The with retry.</returns>
        /// <param name="taskFac">Task factory. We have to use function instead of explicit task for dynamic retrieve new task when retry </param>
        /// <param name="retryMode">Retry mode. default is <see cref="RetryMode.Confirm"/></param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> CallWithRetry<T>(Func<Task<T>> taskFac, RetryMode retryMode = RetryMode.Confirm) where T : new()
        {
            if (taskFac == null)
                throw new ArgumentNullException(nameof(taskFac));
            var result = new T();
            switch (retryMode)
            {
                case RetryMode.None:
                    //execute the api task only
                    try
                    {
                        result = await taskFac.Invoke();
                    }
                    catch (TaskCanceledException ex)
                    {
                        // todo:
                        //result.Result = ApiResult.Canceled;
                        //result.ErrorMessage = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        // todo
                        //result.Result = ApiResult.Failed;
                        //result.ErrorMessage = ex.Message;
                    }
                    break;
                case RetryMode.Warning:
                    var warningRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        await WarningOnMainThread();
                    });
                    result = await warningRetryPolicy.ExecuteAsync(() => ActionSendAsync(taskFac));
                    break;
                case RetryMode.Confirm:
                    var confirmRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        var sure = await ConfirmOnMainThread();
                        if (!sure)
                        {
                            //get the original tokensource passed in execution
                            var orgTcs = context["tokenSource"] as CancellationTokenSource;
                            //cancel it!
                            orgTcs.Cancel();
                        }
                    });
                    var inputTcs = new CancellationTokenSource();
                    try
                    {
                        //passed the token into it to cancel if the user wont choose retry
                        result = await confirmRetryPolicy.ExecuteAsync((inputContext, token) => ActionSendAsync(taskFac), new Context { { "tokenSource", inputTcs } }, inputTcs.Token).ConfigureAwait(true);
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
            //use native handler for better perfomance
            var handler = new ExtendedNativeMessageHandler();
            var toReturn = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiHosts.MainHost),
                Timeout = TimeSpan.FromSeconds(10),
            };
            return toReturn;
        }

        static async Task<T> ActionSendAsync<T>(Func<Task<T>> taskFac) where T : new()
        {
            try
            {
                //precondition by connectivity
                if (!CrossConnectivity.Current.IsConnected)
                    throw new WebException("There's no internet connections!");
                // retrieve the api task from task factory
                var task = taskFac.Invoke();
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
                    //other exception => re-throw
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
        /// Warnings the on main thread. Awaitable
        /// </summary>
        /// <returns>The on main thread.</returns>
        private static Task WarningOnMainThread()
        {
            // use a task completion source for awaitable
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var result = Application.Current.MainPage.DisplayAlert("Warning", "Warning message!", "Ok");//todo: resource
                result.ContinueWith((sender) => tcs.SetResult(true));
            });
            return tcs.Task;
        }

        /// <summary>
        /// Confirms the on main thread. Awaitable
        /// </summary>
        /// <returns>The on main thread.</returns>
        private static Task<bool> ConfirmOnMainThread()
        {
            // use a task completion source for awaitable
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(() =>
            {
                var result = Application.Current.MainPage.DisplayAlert("confirm", "Confirm message?", "Ok", "Cancel");//todo: resource
                result.ContinueWith((sender) => tcs.SetResult(sender.Result));
            });
            return tcs.Task;
        }
        #endregion
    }
}