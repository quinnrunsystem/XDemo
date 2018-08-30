using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XDemo.Core.BusinessServices.Interfaces.Photos
{
    public interface IPhotoService
    {
        Task<List<PhotoDto>> Get();

        Task<PhotoDto> Get(int id);
    }
}
