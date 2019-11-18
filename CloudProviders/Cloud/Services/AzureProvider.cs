// ---------------------------------------------------------------------------
// <copyright file="AzureProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    /// <summary>
    /// Azure Cloud Service Provider
    /// </summary>
    public class AzureProvider : ICloudProvider
    {
        /// <summary>
        /// Azure Cloud Configuration
        /// </summary>
        private AzureConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureProvider" /> class
        /// </summary>
        /// <param name="cloud">Azure cloud configuration</param>
        public AzureProvider(AzureConfiguration cloud)
        {
            this.cloud = cloud;
            this.cloud.LogConfiguration();
        }

        /// <summary>
        /// Gets the Azure storage provider
        /// </summary>
        /// <returns>Azure storage provider</returns>
        public IStorageProvider StorageProvider()
        {
            return new AzureStorageProvider(this.cloud);
        }

        /// <summary>
        /// Gets the Azure queue provider
        /// </summary>
        /// <returns>Azure queue provider</returns>
        public IQueueProvider QueueProvider()
        {
            return new AzureQueueProvider(this.cloud);
        }

        /// <summary>
        /// Gets the Azure cloud auth middleware
        /// </summary>
        /// <returns>Azure cloud auth middleware</returns>
        public IAuthMiddleware AuthMiddleware()
        {
            return new AzureAuthMiddleware(this.cloud);
        }
    }
}
