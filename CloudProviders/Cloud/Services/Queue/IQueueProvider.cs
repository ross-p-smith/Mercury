// ---------------------------------------------------------------------------
// <copyright file="IQueueProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System.Threading.Tasks;

    /// <summary>
    /// Cloud Queue Service Provider Interface
    /// </summary>
    public interface IQueueProvider
    {
        /// <summary>
        /// Asynchronously adds a metadata payload to cloud queue
        /// </summary>
        /// <param name="metadata">metadata to add to queue</param>
        /// <returns>success of operation</returns>
        Task<bool> Enqueue(QueueMetadata metadata);

        /// <summary>
        /// Gets next item from queue
        /// </summary>
        /// <returns>Queue item</returns>
        Task<QueueMetadata> Dequeue();
    }
}