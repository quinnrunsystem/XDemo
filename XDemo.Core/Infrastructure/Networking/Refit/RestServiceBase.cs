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
using XDemo.Core.BusinessServices.Interfaces.Photos;

namespace XDemo.Core.Infrastructure.Networking
{
    public static class RestServiceBase
    {
        #region private methods, executor
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

        static HttpClient GetHttpClient()
        {
            var handler = new ExtendedNativeMessageHandler();
            var toReturn = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiHosts.MainHost),
                Timeout = TimeSpan.FromSeconds(10), //todo: review 10s
            };
            return toReturn;
        }

        public static async Task<T> WrappedExecuteAsync<T>(Task<T> task, RetryMode retryMode = RetryMode.Confirmed) where T : DtoBase, new()
        {
            var result = new T();
            switch (retryMode)
            {
                case RetryMode.None:
                    try
                    {
                        result = await task;
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
                    //todo
                    break;
                case RetryMode.Confirmed:
                    //todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(retryMode), $"The retry mode '{retryMode}' is not supported!");
            }
            return result;
        }

        //public static async Task<T> ConfirmAndRetry<T>(Task<T> task) where T : DtoBase, new()
        //{
        //    // todo: implement all we need retry mode
        //    var response = await Policy.Handle<WebException>()
        //                       .Or<ApiException>()
        //                       .Or<TaskCanceledException>()
        //                               .Or<Exception>()
        //                               //.WaitAndRetryAsync(1, (attemp) => TimeSpan.FromSeconds(1))
        //                               //.RetryAsync((ex, attemp, context)=>{})
        //                               .RetryAsync()
        //                               .ExecuteAsync(() => ActionSendAsync(task));

        //    return response;
        //}

        public static async Task<T> ActionSendAsync<T>(Task<T> task) where T : DtoBase,  new()
        {
            //precondition by connectivity
            if (!CrossConnectivity.Current.IsConnected)
                throw new WebException("There's no internet connections!");

            return await task;
        }
        #endregion
    }
}