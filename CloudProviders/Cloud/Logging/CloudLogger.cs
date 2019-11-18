// ---------------------------------------------------------------------------
// <copyright file="CloudLogger.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// This static class provides the Application Insights Logger to this Library
    /// </summary>
    public static class CloudLogger
    {
        /// <summary>
        /// The App Insights Logger backing field
        /// </summary>
        private static ILogger logger;

        /// <summary>
        /// The App Insights Logger wrapping field
        /// </summary>
        /// <param name="logger"> The logger to be provided to the singleton </param>
        public static void SetLogger(ILogger logger)
        {
            if (CloudLogger.logger == null)
            {
                CloudLogger.logger = logger;
            }
            else
            {
                throw new CloudErrorException("Cloud Logger has already been initialized.");
            }
        }

        /// <summary>
        /// Provides thread safe access to the logger object
        /// </summary>
        /// <param name="logLevel">The level to log the message at</param>
        /// <param name="message">The message to log</param>
        /// <param name="args">The args to pass to the message for string formatting</param>
        public static void Log(LogLevel logLevel, string message, params object[] args)
        {
            if (CloudLogger.logger == null)
            {
                throw new CloudErrorException("Cloud Logger has not been intialized.");
            }

            lock (CloudLogger.logger)
            {
                CloudLogger.logger.Log(logLevel, message, args);
            }
        }
    }
}