using System;
#if DEBUG
using Prism.Logging;
using System.Diagnostics;
#endif

namespace XDemo.Core.Infrastructure.Logging
{
    public class Logger : ILogger
    {
        public void Info(string message)
        {
#if DEBUG
            Debug.WriteLine(BuidlMessage(message, Category.Info));
#endif
        }

        public void Error(string message)
        {
#if DEBUG
            Debug.WriteLine(BuidlMessage(message, Category.Exception));
#endif
        }

        public void Error(Exception exception)
        {
#if DEBUG
            Error(exception?.ToString());
#endif
        }

#if DEBUG
        string BuidlMessage(string rawMsg, Category category)
        {
            return $"[{category.ToString().ToUpper()}][{DateTime.Now.ToString("HH:mm:ss")}] {(string.IsNullOrEmpty(rawMsg) ? "..." : rawMsg)}";
        }
#endif
    }
}
