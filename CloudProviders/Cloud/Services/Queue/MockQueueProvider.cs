// ---------------------------------------------------------------------------
// <copyright file="MockQueueProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    /// Mock queue provider
    /// </summary>
#pragma warning disable 1998
    public class MockQueueProvider : IQueueProvider
    {
        /// <summary>
        /// flag to tell if queuing will succeed.
        /// </summary>
        private bool shouldSuccceed;

        /// <summary>
        /// internal queue
        /// </summary>
        private ConcurrentQueue<QueueMetadata> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockQueueProvider"/> class.
        /// </summary>
        /// <param name="shouldSuccceed">Whether queuing should succeed.</param>
        public MockQueueProvider(bool shouldSuccceed)
        {
            this.shouldSuccceed = shouldSuccceed;
            this.queue = new ConcurrentQueue<QueueMetadata>();
        }

        /// <summary>
        /// Dequeue an item if operation should succeed
        /// </summary>
        /// <returns>Queue item</returns>
        public Task<QueueMetadata> Dequeue()
        {
            if (this.shouldSuccceed)
            {
                QueueMetadata queueItem;
                if (this.queue.TryDequeue(out queueItem))
                {
                    return Task.FromResult(queueItem);
                }
            }

            return Task.FromResult<QueueMetadata>(null);
        }

        /// <summary>
        /// Mocks adding a metadata payload to cloud queue
        /// </summary>
        /// <param name="metadata">metadata to add to queue</param>
        /// <returns>success of operation</returns>
        public async Task<bool> Enqueue(QueueMetadata metadata)
        {
            if (this.shouldSuccceed)
            {
                this.queue.Enqueue(metadata);
            }

            return this.shouldSuccceed;
        }
    }
}