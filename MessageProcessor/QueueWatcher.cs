// ---------------------------------------------------------------------------
// <copyright file="QueueWatcher.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace MessageProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CloudProviders;

    /// <summary>
    /// Queue watcher
    /// </summary>
    public class QueueWatcher
    {
        /// <summary>
        /// Local cloud provider
        /// </summary>
        private ICloudProvider cloud;

        /// <summary>
        /// Amount of time to sleep between polls
        /// </summary>
        private int sleepTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueWatcher"/> class.
        /// </summary>
        /// <param name="cloudProvider">cloud provider</param>
        /// <param name="sleepTime">Time between polls</param>
        public QueueWatcher(ICloudProvider cloudProvider, int sleepTime)
        {
            this.cloud = cloudProvider;
            this.sleepTime = sleepTime;
        }

        /// <summary>
        /// Dequeues a message when it becomes available
        /// </summary>
        /// <returns>Queue item</returns>
        internal async Task<QueueMetadata> DequeueAsync()
        {
            var queueProvider = this.cloud.QueueProvider();
            QueueMetadata queueItem;
            while ((queueItem = await queueProvider?.Dequeue()) == null)
            {
                Thread.Sleep(this.sleepTime);
            }

            return queueItem;
        }

        /// <summary>
        /// Processes the next dequeued item
        /// </summary>
        /// <param name="process">process action</param>
        /// <returns>Async task</returns>
        internal async Task ProcessNext(Func<QueueMetadata, Task> process)
        {
            await process(await this.DequeueAsync());
        }
    }
}