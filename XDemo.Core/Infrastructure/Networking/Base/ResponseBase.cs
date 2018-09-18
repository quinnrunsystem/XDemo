namespace XDemo.Core.Infrastructure.Networking.Base
{
    public class ResponseBase
    {
        public ApiResult Result { get; private set; }
        /// <summary>
        /// error message from server
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// return an Ok result
        /// </summary>
        /// <returns>The ok.</returns>
        public static ResponseBase Ok() => new ResponseBase { Result = ApiResult.Ok };
        /// <summary>
        /// return a failed result with message
        /// </summary>
        /// <returns>The failed.</returns>
        /// <param name="errorMsg">Error message.</param>
        public static ResponseBase Failed(string errorMsg) => new ResponseBase { Result = ApiResult.Failed, ErrorMessage = errorMsg };
        /// <summary>
        /// return a canceled result
        /// </summary>
        /// <returns>The canceled.</returns>
        public static ResponseBase Canceled() => new ResponseBase { Result = ApiResult.Canceled };
    }
}
