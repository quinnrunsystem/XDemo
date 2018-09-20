using System;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Dtos.Photos;
using XDemo.Core.BusinessServices.Dtos.Posts;

namespace XDemo.Core.BusinessServices.Interfaces.Posts
{
    public interface IPostService
    {
        Task<PostDto> CreatePost(CreatePostRequest request);
    }
}
