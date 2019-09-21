﻿namespace DependencyInjectionWorkshop.Models
{
    public class NLogger
    {
        public void LogMessage(string message)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }
    }
}