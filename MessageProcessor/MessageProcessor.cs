// ---------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace MessageProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using CloudProviders;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Executes logic to watch queue and handle notifications
    /// </summary>
    public static class MessageProcessor
    {
        /// <summary>
        /// Runs MessageProcessor
        /// </summary>
        /// <returns>Async task</returns>"
        public static async Task Run()
        {
            var cloud = new CloudFactory(MessageProcessor.GetConfiguration());
            var provider = cloud.Provider();
            QueueWatcher watcher = new QueueWatcher(provider, Constants.SleepTime);
            NotificationHandler handler = new NotificationHandler(provider);

            await MessageProcessor.Run(watcher, handler);
        }

        /// <summary>
        /// Runs message processor
        /// </summary>
        /// <param name="watcher">Queue watcher</param>
        /// <param name="handler">Notification handler</param>
        /// <returns>async task</returns>
        public static async Task Run(QueueWatcher watcher, NotificationHandler handler)
        {
            // Only process one single item, then conclude
            await watcher.ProcessNext(handler.Process);
        }

        /// <summary>
        /// Gets configuration
        /// </summary>
        /// <returns>app settings configuration</returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
