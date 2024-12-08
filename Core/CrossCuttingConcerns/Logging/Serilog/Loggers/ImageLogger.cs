using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class ImageLogger : LoggerServiceBase
    {
        public ImageLogger()
        {
            var configuration = ServiceTool.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            
            if (configuration == null)
            {
                throw new Exception("Configuration service not found");
            }

            string logFilePath;
            try 
            {
                logFilePath = configuration.GetSection("ImageLogPath").Value ?? "Logs/image-log-.txt";
            }
            catch
            {
                logFilePath = "Logs/image-log-.txt";
            }

            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            Logger = new LoggerConfiguration()
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 5242880,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
        }
    }
} 