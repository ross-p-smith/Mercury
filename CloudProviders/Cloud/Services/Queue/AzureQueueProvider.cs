// ---------------------------------------------------------------------------
// <copyright file="AzureQueueProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Queue;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Queue Provider
    /// </summary>
    public class AzureQueueProvider : IQueueProvider
    {
        /// <summary>
        /// Azure Storage Queue
        /// </summary>
        private CloudQueue cloudQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueProvider" /> class
        /// </summary>
        /// <param name="cloud">Azure cloud configuration</param>
        public AzureQueueProvider(AzureConfiguration cloud)
        {
            var storageAccount = cloud.GetStorageAccountAsync().Result;
            if (storageAccount == null)
            {
                throw new Exception("Azure Queue provider can't find storage account");
            }

            var queueClient = storageAccount.CreateCloudQueueClient();
            this.cloudQueue = queueClient.GetQueueReference(cloud.QueueName);
            this.cloudQueue.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Asynchronously adds a metadata payload to Azure storage queue
        /// </summary>
        /// <param name="metadata">metadata to add to queue</param>
        /// <returns>success of operation</returns>
        public async Task<bool> Enqueue(QueueMetadata metadata)
        {
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(metadata));
            await this.cloudQueue.AddMessageAsync(message);
            return true;
        }

        /// <summary>
        /// Dequeue a queue item
        /// </summary>
        /// <returns>Queue item</returns>
        public async Task<QueueMetadata> Dequeue()
        {
            if (await this.cloudQueue.ExistsAsync())
            {
                CloudQueueMessage message = await this.cloudQueue.GetMessageAsync();
                if (message != null)
                {
                    string messageContent = message.AsString;
                    await this.cloudQueue.DeleteMessageAsync(message);
                    return JsonConvert.DeserializeObject<QueueMetadata>(messageContent);
                }

                return null;
            }

            throw new Exception("Queue doesn't exist");
        }
    }
}
