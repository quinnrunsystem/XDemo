using System;
using System.Diagnostics;
using Prism.Logging;
namespace XDemo.Core.Infrastructure.Logging
{
    public class Logger : ILogger
    {
        private readonly ILoggerFacade _loggerFacade;

        public Logger(ILoggerFacade loggerFacade)
        {
            // todo: now we use prism built-in logger. In the future we can implement this by another
            _loggerFacade = loggerFacade;
        }
        public void Info(string message, Priority priority)
        {
            _loggerFacade.Log(message, Category.Info, priority);
        }

        public void Error(string message)
        {
            _loggerFacade.Log(message, Category.Exception, Priority.High);
        }

        public void Error(Exception exception)
        {
            Error(exception?.ToString());
        }
    }
}
