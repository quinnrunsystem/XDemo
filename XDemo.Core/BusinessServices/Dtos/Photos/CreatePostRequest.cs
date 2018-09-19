using XDemo.Core.Infrastructure.Networking.Base;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace XDemo.Core.BusinessServices.Dtos.Photos
{
    public class CreatePostRequest : RequestBase
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}
