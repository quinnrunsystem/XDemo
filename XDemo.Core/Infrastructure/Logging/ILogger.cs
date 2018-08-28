using System;

namespace XDemo.Core.Infrastructure.Logging
{
    public interface ILogger
    {
        void Info(string message);

        void Error(string errorMessage);

        void Error(Exception exception);
    }
}
