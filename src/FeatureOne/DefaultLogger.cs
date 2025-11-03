using System;
using Microsoft.Extensions.Logging;

namespace FeatureOne
{
    public class DefaultLogger : IFeatureLogger
    {
        private readonly ILogger<IFeatureLogger> logger;

        public DefaultLogger(ILogger<IFeatureLogger> logger)
        {
            this.logger = logger;
        }
        public void Info(string message)
        {
            logger?.LogInformation(message);
        }

        public void Debug(string message)
        {
            logger?.LogDebug(message);

        }

        public void Warn(string message)
        {
            logger?.LogWarning(message);
        }

        public void Error(string message, Exception ex)
        {
            logger?.LogError(ex, message);
        }
    }
}