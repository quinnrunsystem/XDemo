using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace XDemo.Core.BusinessServices.Interfaces.Photos
{
    public interface IPhotoService
    {
        Task Get(Action<List<PhotoDto>> onSuccessAction, Action onFailedAction, CancellationToken extToken = default(CancellationToken));

        Task<PhotoDto> Get(int id);

        Task<PhotoDto> GetWithSilentRetryUntilSuccess(int id);
    }
}
