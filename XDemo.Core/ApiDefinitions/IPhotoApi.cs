using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using System.Threading;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace XDemo.Core.ApiDefinitions
{
    public interface IPhotoApi
    {
        /* ==================================================================================================
         * Example for using an api call with a cancellation token
         * Just pass only 1 cancellation parameter into this.
         * ================================================================================================*/
        [Get("/photos")]
        Task<List<PhotoDto>> Get(CancellationToken token);

        /* ==================================================================================================
         * Example for simple GET method
         * ================================================================================================*/
        [Get("/photos/{id}")]
        Task<PhotoDto> Get(int id);
    }
}
