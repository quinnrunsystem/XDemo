using System;
#if DEBUG
using Prism.Logging;
using System.Diagnostics;
#endif

namespace XDemo.Core.Infrastructure.Logging
{
    public static class LogCommon
    {
        public static void Info(string message)
        {
#if DEBUG
            Debug.WriteLine(BuidlMessage(message, Category.Info));
#endif
        }

        public static void Error(string message)
        {
#if DEBUG
            Debug.WriteLine(BuidlMessage(message, Category.Exception));
#endif
        }

        public static void Error(Exception exception)
        {
#if DEBUG
            Error(exception?.ToString());
#endif
        }

#if DEBUG
        static string BuidlMessage(string rawMsg, Category category)
        {
            return $"[{DateTime.Now.ToString("HH:mm:ss")}][{category.ToString().ToUpper()}] {(string.IsNullOrEmpty(rawMsg) ? "..." : rawMsg)}";
        }
#endif
    }
}
