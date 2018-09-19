namespace XDemo.Core.Infrastructure.Networking.Base
{
    public abstract class RequestBase
    {
        /* ==================================================================================================
         * sample usage of base property 'Token'
         * Readonly mode
         * ================================================================================================*/
        public string Token => _token;

        private static string _token;
        public static void SetToken(string token)
        {
            /* ==================================================================================================
             * store the token in a backing static store
             * ================================================================================================*/
            _token = token;
        }
    }
}
