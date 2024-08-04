using Microsoft.Extensions.Logging;

namespace Shuttle.ProcessManagement.Logging
{
    public class FileLogger : ILogger, IDisposable
    {
        private static readonly object Lock = new object();
        private readonly StreamWriter _stream;
        private DateTime _previousLogDateTime = DateTime.MinValue;

        public FileLogger(string name)
        {
            const string rootFolderName = "Shuttle.ProcessManagement";
            var folder = Path.Combine(AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf(rootFolderName) + rootFolderName.Length), ".logs");
            var path = Path.Combine(folder, $"{name}--{DateTime.Now:yyy-MM-dd--HH-mm-ss.fff}.log");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _stream = new StreamWriter(path);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (Lock)
            {
                var now = DateTime.Now;

                _stream.WriteLine($"{now:HH:mm:ss.fffffff} / {(_previousLogDateTime > DateTime.MinValue ? $"{(now - _previousLogDateTime):fffffff}" : "0000000")} - {formatter(state, exception)}");
                _stream.Flush();

                _previousLogDateTime = now;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return default!;
        }
    }
}