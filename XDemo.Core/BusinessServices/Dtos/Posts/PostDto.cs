using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace XDemo.Core.BusinessServices.Dtos.Posts
{
    public class PostDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }
    }
}
