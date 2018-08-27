using System;
using Prism.Logging;

namespace XDemo.Core.Infrastructure.Logging
{
    public interface ILogger
    {
        void Info(string message, Priority priority = Priority.None);

        void Error(string errorMessage);

        void Error(Exception exception);
    }
}
