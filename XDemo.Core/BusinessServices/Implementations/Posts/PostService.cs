using System;
using System.Threading.Tasks;
using XDemo.Core.BusinessServices.Interfaces.Posts;
using XDemo.Core.Infrastructure.Networking.Refit;
using XDemo.Core.ApiDefinitions;
using XDemo.Core.BusinessServices.Dtos.Posts;
using XDemo.Core.Extensions;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace XDemo.Core.BusinessServices.Implementations.Posts
{
    public class PostService : IPostService
    {
        private readonly IPostsApi _postsApi;
        public PostService(IPostsApi postsApi)
        {
            _postsApi = postsApi;
        }

        public async Task<PostDto> CreatePost(PostDto dto)
        {
            /* ==================================================================================================
             * map dto to request for api call
             * ================================================================================================*/
            var request = dto.MapTo<CreatePostRequest>();
            /* ==================================================================================================
             * action call
             * ================================================================================================*/
            var pair = await RestServiceHelper.CallWithRetry(() => _postsApi.Create(request));
            /* ==================================================================================================
             * exp dont care about extended result
             * ================================================================================================*/
            return pair.MainResult;
        }
    }
}
