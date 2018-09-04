using System;
using System.Collections.Generic;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using System.Threading.Tasks;
using Refit;
using System.Threading;
using System.Net.Http;
using XDemo.Core.Infrastructure.Networking.Base;

namespace XDemo.Core.ApiDefinitions
{
    //[Headers("Content-Type: application/json; charset=UTF-8")]
    public interface IPhotoApi
    {
        [Get("/photos")]
        Task<ListDtoBase<PhotoDto>> Get(CancellationToken token);

        [Get("/photos/{id}")]
        Task<PhotoDto> Get(int id);
    }
}
