using System.Collections.Generic;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Infrastructure.Networking;
using XDemo.Core.ApiDefinitions;
using System.Threading;
using System;
using XDemo.Core.Infrastructure.Networking.Base;
namespace XDemo.Core.BusinessServices.Implementations.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoApi _photoApi;
        public PhotoService()
        {
            _photoApi = RestServiceHelper.GetApi<IPhotoApi>();
        }

        public async Task<ListDtoBase<PhotoDto>> Get(CancellationToken extToken)
        {
            var api = RestServiceHelper.GetApi<IPhotoApi>();
            return await RestServiceHelper.CallWithRetry(api.Get(extToken), RetryMode.Confirm);
        }

        public async Task<PhotoDto> Get(int id)
        {
            var api = RestServiceHelper.GetApi<IPhotoApi>();
            return await RestServiceHelper.CallWithRetry(api.Get(id), RetryMode.Confirm);
        }
    }
}
