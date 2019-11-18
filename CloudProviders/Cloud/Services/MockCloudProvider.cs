// ---------------------------------------------------------------------------
// <copyright file="MockCloudProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;

    /// <summary>
    /// Mock cloud provider
    /// </summary>
    public class MockCloudProvider : ICloudProvider
    {
        /// <summary>
        /// flag to tell if the child providers will succeed.
        /// </summary>
        private bool shouldSuccceed;

        /// <summary>
        /// Queue provider
        /// </summary>
        private IQueueProvider queueProvider;

        /// <summary>
        /// Storage provider
        /// </summary>
        private IStorageProvider storageProvider;

        /// <summary>
        /// Auth middleware
        /// </summary>
        private IAuthMiddleware authMiddleware;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockCloudProvider"/> class.
        /// </summary>
        /// <param name="shouldSuccceed">Whether upload should succeed.</param>
        public MockCloudProvider(bool shouldSuccceed)
        {
            this.shouldSuccceed = shouldSuccceed;
            this.queueProvider = new MockQueueProvider(this.shouldSuccceed);
            this.storageProvider = new MockStorageProvider(this.shouldSuccceed);
            this.authMiddleware = new MockAuthMiddleware();
            Console.WriteLine("Using MockCloudProvider");
        }

        /// <summary>
        /// Gets the cloud storage provider
        /// </summary>
        /// <returns>cloud storage provider</returns>
        public IQueueProvider QueueProvider()
        {
            return this.queueProvider;
        }

        /// <summary>
        /// Gets the cloud storage provider
        /// </summary>
        /// <returns>cloud storage provider</returns>
        public IStorageProvider StorageProvider()
        {
            return this.storageProvider;
        }

        /// <summary>
        /// Gets the cloud auth middleware
        /// </summary>
        /// <returns>cloud storage provider</returns>
        public IAuthMiddleware AuthMiddleware()
        {
            return this.authMiddleware;
        }
    }
}