using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using XDemo.Core.Infrastructure.Networking.Base;

namespace XDemo.Core.BusinessServices.Interfaces.Photos
{
    public interface IPhotoService
    {
        Task<ListDtoBase<PhotoDto>> Get(CancellationToken extToken = default(CancellationToken));

        Task<PhotoDto> Get(int id);
    }
}
