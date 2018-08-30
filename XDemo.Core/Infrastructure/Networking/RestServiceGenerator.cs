using Refit;
using System.Net.Http;
using System;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.Core.ApiDefinitions;

namespace XDemo.Core.Infrastructure.Networking
{
    public static class RestServiceGenerator
    {
        public static TApiInterface For<TApiInterface>()
        {
            // set default refit setting
            var defaultSettings = new RefitSettings
            {
                JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                }
            };
            return RestService.For<TApiInterface>(GetHttpClient(), defaultSettings);
        }

        static HttpClient GetHttpClient()
        {
            var handler = new ExtendedHttpClientHandler();
            var toReturn = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiHosts.MainHost),
                Timeout = TimeSpan.FromSeconds(10),//todo: review 10s
            };
            return toReturn;
        }
    }
}