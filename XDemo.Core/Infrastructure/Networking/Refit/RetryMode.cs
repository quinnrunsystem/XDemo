namespace XDemo.Core.Infrastructure.Networking.Refit
{
    public enum RetryMode
    {
        /// <summary>
        /// Dont retry, just return the result only
        /// </summary>
        None,
        /// <summary>
        /// Show warning then retry
        /// </summary>
        Warning,
        /// <summary>
        /// confirmed then retry
        /// </summary>
        Confirm,
        /// <summary>
        /// retry in silent! <para/>
        /// Exp usage: send the push notification token to the server, it must be succeeded!!!
        /// </summary>
        SilentUntilSuccess
        /* ==================================================================================================
         * Define more retry mode here if needed!
         * ================================================================================================*/
    }
}