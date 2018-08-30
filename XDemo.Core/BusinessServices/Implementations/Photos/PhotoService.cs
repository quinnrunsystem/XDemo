using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Infrastructure.Networking;
using XDemo.Core.ApiDefinitions;
using System.Threading;
namespace XDemo.Core.BusinessServices.Implementations.Photos
{
    public class PhotoService : IPhotoService
    {
        public async Task<List<PhotoDto>> Get()
        {
            var api = RestServiceGenerator.For<IPhotoApi>();
            var tcs = new CancellationTokenSource();
            tcs.CancelAfter(10000);
            return await api.Get(tcs.Token);
        }

        public async Task<PhotoDto> Get(int id)
        {
            var api = RestServiceGenerator.For<IPhotoApi>();
            return await api.Get(id);
        }
    }
}
