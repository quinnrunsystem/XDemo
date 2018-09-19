using System;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Dtos.Photos;
using XDemo.Core.BusinessServices.Interfaces.Posts;
using XDemo.Core.Infrastructure.Networking.Refit;
using XDemo.Core.ApiDefinitions;

namespace XDemo.Core.BusinessServices.Implementations.Posts
{
    public class PostService:IPostService
    {
        private readonly IPostsApi _postsApi;
        public PostService(IPostsApi postsApi)
        {
            _postsApi = postsApi;
        }

        public async Task<PhotoDto> CreatePost(CreatePostRequest request)
        {
            var pair = await RestServiceHelper.CallWithRetry(() => _postsApi.Create(request));
            /* ==================================================================================================
             * exp dont care about extended result
             * ================================================================================================*/
            return pair.MainResult;
        }
    }
}
