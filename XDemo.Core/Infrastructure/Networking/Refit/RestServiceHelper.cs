using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Refit;
using XDemo.Core.ApiDefinitions;
using XDemo.Core.Infrastructure.Logging;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.Core.Shared;
using Prism.Services;
using Xamarin.Forms;
using Polly.Timeout;

namespace XDemo.Core.Infrastructure.Networking.Refit
{
    public static class RestServiceHelper
    {
        public const int ApiCallTimeoutInSeconds = 10;

        public static TApi GetApi<TApi>()
        {
            /* ==================================================================================================
             * set default refit setting
             * ie: ignore null field from json string
             * ================================================================================================*/
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
        /// Calls the api task with retry. <para/>
        /// MainResult: the expected result of api call <para/>
        /// ExtendedResult: the extended result of api call, use this for your manual logic handler
        /// </summary>
        /// <returns>The with retry.</returns>
        /// <param name="taskFac">Task factory. We have to use function instead of explicit task for dynamic retrieve new task when retry </param>
        /// <param name="retryMode">Retry mode. default is <see cref="RetryMode.Confirm"/></param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<(T MainResult, ResponseBase ExtendedResult)> CallWithRetry<T>(Func<Task<T>> taskFac, RetryMode retryMode = RetryMode.Confirm) where T : new()
        {
            if (taskFac == null)
                throw new ArgumentNullException(nameof(taskFac));

            switch (retryMode)
            {
                case RetryMode.None:
                    try
                    {
                        /* ==================================================================================================
                         * execute the api task only, but dont thrown any exception
                         * ================================================================================================*/
                        var mainResult = await taskFac.Invoke();
                        return (mainResult, ResponseBase.Ok());
                    }
                    catch (OperationCanceledException ex)
                    {
                        /* ==================================================================================================
                         * ignored: the api inner call canceled by the passed token
                         * Return a new instance of T to avoid another NullReferenceException!
                         * ================================================================================================*/
                        LogCommon.Error(ex);
                        return (new T(), ResponseBase.Canceled());
                    }
                    catch (Exception ex)
                    {
                        LogCommon.Error(ex);
                        return (new T(), ResponseBase.Failed(ex.Message));
                    }
                case RetryMode.Warning:
                    var warningRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        /* ==================================================================================================
                         * In some case, we need to call api in background. So, we need to call UIThread display alert message
                         * todo: using resource for alert message
                         * ================================================================================================*/
                        var dialogService = DependencyRegistrar.Current.Resolve<IPageDialogService>();
                        await ThreadHelper.RunOnUIThreadAsync(() => dialogService.DisplayAlertAsync("Warning", "Warning message!", "Ok"));
                    });
                    var result = await warningRetryPolicy.ExecuteAsync(() => ActionSendAsync(taskFac));
                    return result;
                case RetryMode.Confirm:
                    var confirmRetryPolicy = Policy.Handle<Exception>().RetryForeverAsync(async (exception, retryCount, context) =>
                    {
                        LogCommon.Error($"retry no {retryCount} - Exception msg: {exception.Message}");
                        /* ==================================================================================================
                         * In some case, we need to call api in background. So, we need to call UIThread display alert message
                         * todo: using resource for alert message
                         * ================================================================================================*/
                        var dialogService = DependencyRegistrar.Current.Resolve<IPageDialogService>();
                        var sure = await ThreadHelper.RunOnUIThreadAsync(() => dialogService.DisplayAlertAsync("confirm", "Confirm message?", "Ok", "Cancel"));
                        if (!sure)
                        {
                            /* ==================================================================================================
                             * get the original tokensource passed in execution before and cancel it to break the retry cycle!
                             * ================================================================================================*/
                            var orgTcs = context["tokenSource"] as CancellationTokenSource;
                            orgTcs?.Cancel();
                        }
                    });
                    var inputTcs = new CancellationTokenSource();
                    try
                    {
                        /* ==================================================================================================
                         * passed the token into it to cancel if the user wont choose 'retry'
                         * ================================================================================================*/
                        var toReturn = await confirmRetryPolicy.ExecuteAsync((inputContext, token) => ActionSendAsync(taskFac), new Context { { "tokenSource", inputTcs } }, inputTcs.Token).ConfigureAwait(false);
                        return toReturn;
                    }
                    catch (OperationCanceledException ex)
                    {
                        /* ==================================================================================================
                         * ignore: the retry cycle broken bc the user didn't retry => return as a failed result
                         * ================================================================================================*/
                        LogCommon.Error(ex);
                        return (new T(), ResponseBase.Failed("User dont want to retry anymore!"));
                    }
                default:
                    /* ==================================================================================================
                     * the retry mode is not support yet!
                     * ================================================================================================*/
                    throw new ArgumentOutOfRangeException(nameof(retryMode), $"The retry mode '{retryMode}' is not supported yet!");
            }
        }

        #region private methods, executor
        static HttpClient GetHttpClient()
        {
            /* ==================================================================================================
             * Use native handler for better perfomance
             * For Android v4.4 vs ModernHttp compability => Work as expected on this version
             * ================================================================================================*/
            var handler = new ExtendedNativeMessageHandler()
            {
                /* ==================================================================================================
                 * from SYSFX experience: dont allow api call store cache data (request/response) in cache db.
                 * if caching was enabled: our data can be read from outside!
                 * we have to use this package to avoid some deadlock cases of built-in httpclient
                 * ================================================================================================*/
                DisableCaching = true
            };

            var toReturn = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiHosts.MainHost),
                /* ==================================================================================================
                 * DO NOT SET TIMEOUT FOR THE HttpClient => use TimeoutPolicy instead
                 * ================================================================================================*/
            };
            return toReturn;
        }

        /* ==================================================================================================
         * Why use Func<Task<T>> instead of an implicit task?
         * => 
         * In case of the api task failed by network connection lost, its still has faulted status forever.
         * Use an Func<> to retrieve a new task for each retry
         * ================================================================================================*/
        static async Task<(T MainResult, ResponseBase ExtendedResult)> ActionSendAsync<T>(Func<Task<T>> taskFac) where T : new()
        {
            var timeoutPolicy = Policy.TimeoutAsync(ApiCallTimeoutInSeconds, TimeoutStrategy.Pessimistic);
            try
            {
                /* ==================================================================================================
                 * retrieve the api task from task factory
                 * ================================================================================================*/
                var mainResult = await timeoutPolicy.ExecuteAsync(taskFac);
                return (mainResult, ResponseBase.Ok());
            }
            catch (TimeoutRejectedException ex)
            {
                /* ==================================================================================================
                 * rethrow the System.TimeoutException instead of a third-party exception!
                 * ================================================================================================*/
                throw new TimeoutException(ex.Message, ex);
            }
            catch (OperationCanceledException ex)
            {
                LogCommon.Error(ex);
                /* ==================================================================================================
                 * ignored: the api inner call canceled by the passed token of api task
                 * ================================================================================================*/
                return (new T(), ResponseBase.Canceled());
            }
            catch (AggregateException ex)
            {
                if (!(ex.InnerException is OperationCanceledException))
                    /* ==================================================================================================
                     * other exception => re-thrown for polly handler
                     * ================================================================================================*/
                    throw;

                return (new T(), ResponseBase.Canceled());
            }
            catch (Exception ex)
            {
                /* ==================================================================================================
                 * rethrown the exception for polly handler, included timeout
                 * ================================================================================================*/
                throw ex;
            }
        }
        #endregion
    }
}