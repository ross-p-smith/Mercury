// ---------------------------------------------------------------------------
// <copyright file="CloudConfiguration.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;

    /// <summary>
    /// Base Cloud Configuration
    /// </summary>
    public class CloudConfiguration
    {
        /// <summary>
        /// Gets or sets Cloud storage container name
        /// </summary>
        public string StorageFolder { get; set; }

        /// <summary>
        /// Gets or sets Cloud queue name
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Logs configuration
        /// </summary>
        public void LogConfiguration()
        {
            Console.WriteLine("StorageFolder={0}", this.StorageFolder);
            Console.WriteLine("QueueName={0}", this.QueueName);
        }
    }
}
