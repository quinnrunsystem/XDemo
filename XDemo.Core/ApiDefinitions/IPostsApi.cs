using System;
using System.Threading.Tasks;
using Refit;
using XDemo.Core.BusinessServices.Dtos.Photos;
using XDemo.Core.BusinessServices.Dtos.Posts;

namespace XDemo.Core.ApiDefinitions
{
    public interface IPostsApi
    {
        [Post("/posts")]
        Task<PostDto> Create(CreatePostRequest request);
    }
}
