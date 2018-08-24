namespace XDemo.Core.Infrastructure.Networking.Base
{
    public class RequestBase<T>
    {
        /// <summary>
        /// this property will store logged in token, session id... returned by server
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }
        
        public T Body { get; set; }
        //todo: other stuffs
    }
}
