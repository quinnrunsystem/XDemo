using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.ApiDefinitions;
using System.Threading;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.Core.Infrastructure.Networking.Refit;
using System.Collections.Generic;
using XDemo.Core.BusinessServices.Dtos.Photos;
using System;

namespace XDemo.Core.BusinessServices.Implementations.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoApi _photoApi;
        public PhotoService(IPhotoApi photoApi)
        {
            /* ==================================================================================================
             * manual resolve: 
             * _photoApi = RestServiceHelper.GetApi<IPhotoApi>();
             * ================================================================================================*/

            /* ==================================================================================================
             * DI usage
             * ================================================================================================*/
            _photoApi = photoApi;
        }

        /* ==================================================================================================
         * example for action in service usage (farmiliar to sysfx)
         * this way vs handling in viewmodel are same
         * ================================================================================================*/
        public async Task Get(Action<List<PhotoDto>> onSuccessAction, Action onFailedAction, CancellationToken extToken)
        {
            //await Task.Delay(2000);
            var pair = await RestServiceHelper.CallWithRetry(() => _photoApi.Get(extToken), RetryMode.Confirm);
            switch (pair.ExtendedResult.Result)
            {
                case ApiResult.Failed:
                    /* ==================================================================================================
                     * failed api call
                     * ================================================================================================*/
                    onFailedAction?.Invoke();
                    break;
                case ApiResult.Ok:
                    /* ==================================================================================================
                     * success api call
                     * ================================================================================================*/
                    onSuccessAction?.Invoke(pair.MainResult);
                    break;
                case ApiResult.Canceled:
                    /* ==================================================================================================
                     * Do nothing
                     * ================================================================================================*/
                    break;
                default:
                    break;
            }
        }

        public async Task<PhotoDto> Get(int id)
        {
            var pair = await RestServiceHelper.CallWithRetry(() => _photoApi.Get(id), RetryMode.Confirm);
            return pair.MainResult;
        }

        public async Task<PhotoDto> GetWithSilentRetryUntilSuccess(int id)
        {
            var pair = await RestServiceHelper.CallWithRetry(() => _photoApi.Get(id), RetryMode.SilentUntilSuccess);
            return pair.MainResult;
        }
    }
}
