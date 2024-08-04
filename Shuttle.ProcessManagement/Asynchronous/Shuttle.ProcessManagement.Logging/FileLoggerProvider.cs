using Microsoft.Extensions.Logging;

namespace Shuttle.ProcessManagement.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly FileLogger _logger;

        public FileLoggerProvider(string name)
        {
            _logger = new FileLogger(name);
        }

        public void Dispose()
        {
            _logger.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }
    }
}