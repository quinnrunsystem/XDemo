using System;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace XDemo.Core.BusinessServices.Interfaces.Posts
{
    public interface IPostService
    {
        Task<PhotoDto> CreatePost(CreatePostRequest request);
    }
}
