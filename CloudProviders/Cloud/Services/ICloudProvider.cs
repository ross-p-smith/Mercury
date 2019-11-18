// ---------------------------------------------------------------------------
// <copyright file="ICloudProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    /// <summary>
    /// Cloud Service Provider Interface
    /// </summary>
    public interface ICloudProvider
    {
        /// <summary>
        /// Gets the cloud storage provider
        /// </summary>
        /// <returns>cloud storage provider</returns>
        IQueueProvider QueueProvider();

        /// <summary>
        /// Gets the cloud storage provider
        /// </summary>
        /// <returns>cloud storage provider</returns>
        IStorageProvider StorageProvider();

        /// <summary>
        /// Gets the cloud auth middleware
        /// </summary>
        /// <returns>cloud auth middleware</returns>
        IAuthMiddleware AuthMiddleware();
    }
}
