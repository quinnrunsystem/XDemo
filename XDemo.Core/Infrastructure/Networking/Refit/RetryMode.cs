namespace XDemo.Core.Infrastructure.Networking
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
        Confirm
    }
}