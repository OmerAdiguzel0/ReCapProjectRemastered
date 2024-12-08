using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Serilog
{
    public abstract class LoggerServiceBase
    {
        protected ILogger Logger { get; set; }

        public void Info(string message)
        {
            Logger.Information(message);
        }

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void Warning(string message)
        {
            Logger.Warning(message);
        }

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }
    }
} 