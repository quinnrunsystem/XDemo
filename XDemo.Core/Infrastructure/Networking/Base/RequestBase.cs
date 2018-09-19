using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace XDemo.Core.Infrastructure.Networking.Base
{
    public abstract class RequestBase
    {
        /* ==================================================================================================
         * sample usage of base property 'Token'
         * Readonly mode
         * ================================================================================================*/
        [JsonProperty("sessionId")]
        public string Token => _sessionId;

        private static string _sessionId;
        public static void SessionId(string session)
        {
            /* ==================================================================================================
             * store the token in a backing static store
             * ================================================================================================*/
            _sessionId = session;
        }
    }
}
