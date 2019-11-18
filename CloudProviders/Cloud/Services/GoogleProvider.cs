// ---------------------------------------------------------------------------
// <copyright file="GoogleProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    /// <summary>
    /// Google Cloud services provider
    /// </summary>
    public class GoogleProvider : ICloudProvider
    {
        /// <summary>
        /// Google cloud configuration
        /// </summary>
        private GoogleConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleProvider" /> class
        /// </summary>
        /// <param name="cloud">Azure cloud configuration</param>
        public GoogleProvider(GoogleConfiguration cloud)
        {
            this.cloud = cloud;
            this.cloud.LogConfiguration();
        }

        /// <summary>
        /// Gets the Google storage provider
        /// </summary>
        /// <returns>Google storage provider</returns>
        public IStorageProvider StorageProvider()
        {
            return new GoogleStorageProvider(this.cloud);
        }

        /// <summary>
        /// Gets the Google queue provider
        /// </summary>
        /// <returns>Google queue provider</returns>
        public IQueueProvider QueueProvider()
        {
            return new GoogleQueueProvider(this.cloud);
        }

        /// <summary>
        /// Gets the Google cloud auth middleware
        /// </summary>
        /// <returns>Google cloud auth middleware</returns>
        public IAuthMiddleware AuthMiddleware()
        {
            return new GoogleAuthMiddleware(this.cloud);
        }
    }
}
