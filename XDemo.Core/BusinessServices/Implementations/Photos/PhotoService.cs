using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Infrastructure.Networking;
using XDemo.Core.ApiDefinitions;
using System.Threading;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.Core.Infrastructure.Networking.Refit;
using System.Collections.Generic;

namespace XDemo.Core.BusinessServices.Implementations.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoApi _photoApi;
        public PhotoService()
        {
            _photoApi = RestServiceHelper.GetApi<IPhotoApi>();
        }

        public async Task<List<PhotoDto>> Get(CancellationToken extToken)
        {
            return await RestServiceHelper.CallWithRetry(() => _photoApi.Get(extToken), RetryMode.Confirm);
        }

        public async Task<PhotoDto> Get(int id)
        {
            return await RestServiceHelper.CallWithRetry(() => _photoApi.Get(id), RetryMode.Confirm);
        }
    }
}
