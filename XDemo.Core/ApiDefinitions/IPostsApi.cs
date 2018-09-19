using System;
using System.Threading.Tasks;
using Refit;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace XDemo.Core.ApiDefinitions
{
    public interface IPostsApi
    {
        [Post("/posts")]
        Task<PhotoDto> Create(CreatePostRequest request);
    }
}
