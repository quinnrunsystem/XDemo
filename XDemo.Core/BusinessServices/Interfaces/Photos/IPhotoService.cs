using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using XDemo.Core.BusinessServices.Dtos.Photos;
using XDemo.Core.Infrastructure.Networking.Base;

namespace XDemo.Core.BusinessServices.Interfaces.Photos
{
    public interface IPhotoService
    {
        Task Get(Action<List<PhotoDto>> onSuccessAction, Action onFailedAction, CancellationToken extToken = default(CancellationToken));

        Task<PhotoDto> Get(int id);
    }
}
