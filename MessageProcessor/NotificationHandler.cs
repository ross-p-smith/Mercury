// ---------------------------------------------------------------------------
// <copyright file="NotificationHandler.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace MessageProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using CloudProviders;
    using Newtonsoft.Json;

    /// <summary>
    /// Notification handler for queue items
    /// </summary>
    public class NotificationHandler
    {
        /// <summary>
        /// Cloud provider
        /// </summary>
        private ICloudProvider cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHandler"/> class.
        /// </summary>
        /// <param name="cloudProvider">cloud provider</param>
        public NotificationHandler(ICloudProvider cloudProvider)
        {
            this.cloud = cloudProvider;
        }

        /// <summary>
        /// Processes item by writing a new file
        /// </summary>
        /// <param name="item">Queue item</param>
        /// <returns>Async task</returns>
        internal async Task Process(QueueMetadata item)
        {
           await this.cloud.StorageProvider().UploadFile(
                Constants.NotifyFilename,
                new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(item))));
        }
    }
}
