// ---------------------------------------------------------------------------
// <copyright file="GoogleQueueProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Api.Gax.ResourceNames;
    using Google.Cloud.PubSub.V1;
    using Google.Protobuf;
    using Grpc.Core;
    using Newtonsoft.Json;

    /// <summary>
    /// Google Cloud Queue Provider
    /// </summary>
    public class GoogleQueueProvider : IQueueProvider
    {
        /// <summary>
        /// Google cloud configuration
        /// </summary>
        private GoogleConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleQueueProvider" /> class
        /// </summary>
        /// <param name="cloud">Google Cloud configuration</param>
        public GoogleQueueProvider(GoogleConfiguration cloud)
        {
            this.cloud = cloud;
            this.SubscriptionName = this.CreateSubscriptionIfNotExists();
            this.SubscriptionClient = SubscriberServiceApiClient.Create();
        }

        /// <summary>
        /// Google pubsub subscription name
        /// </summary>
        public SubscriptionName SubscriptionName { get; private set; }

        /// <summary>
        /// Google pubsub subscription client
        /// </summary>
        public SubscriberServiceApiClient SubscriptionClient { get; private set; }

        /// <summary>
        /// Asynchronously adds a metadata payload to Google PubSub
        /// </summary>
        /// <param name="metadata">metadata to add to queue</param>
        /// <returns>success of operation</returns>
        public async Task<bool> Enqueue(QueueMetadata metadata)
        {
            try
            {
                PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();

                TopicName topicName = new TopicName(this.cloud.GoogleProjectID, this.cloud.QueueName);
                await this.CreateTopicIfNotExistsAsync(publisher, topicName);

                var message = new PubsubMessage()
                {
                    Data = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(metadata)),
                };

                var messageList = new List<PubsubMessage>() { message };

                await publisher.PublishAsync(topicName, messageList);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Dequeue an item
        /// </summary>
        /// <returns>item from queue</returns>
        public async Task<QueueMetadata> Dequeue()
        {
            PullResponse response = await this.SubscriptionClient.PullAsync(this.SubscriptionName, returnImmediately: true, maxMessages: 1);

            foreach (ReceivedMessage msg in response.ReceivedMessages)
            {
                string text = Encoding.UTF8.GetString(msg.Message.Data.ToArray());
                this.SubscriptionClient.Acknowledge(this.SubscriptionName, response.ReceivedMessages.Select(m => m.AckId));
                Console.WriteLine($"Message {msg.Message.MessageId}: {text}");

                // only process a single message at a time
                return JsonConvert.DeserializeObject<QueueMetadata>(text);
            }

            return null;
        }

        /// <summary>
        /// Creates a topic subscription if doesnt exist
        /// </summary>
        /// <returns>Subscription name</returns>
        private SubscriptionName CreateSubscriptionIfNotExists()
        {
            SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
            TopicName topicName = new TopicName(this.cloud.GoogleProjectID, this.cloud.QueueName);
            SubscriptionName subscriptionName = new SubscriptionName(this.cloud.GoogleProjectID, this.cloud.GoogleSubscriptionName);
            try
            {
                _ = subscriber.CreateSubscription(
                    subscriptionName,
                    topicName,
                    pushConfig: null,
                    ackDeadlineSeconds: 60);
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // It already exists
            }

            return subscriptionName;
        }

        /// <summary>
        /// Asynchronously creates a Google PubSub topic if it does not exist
        /// </summary>
        /// <param name="publisher">publisher service</param>
        /// <param name="topicName">topic name</param>
        /// <returns>A Task containing the RPC response</returns>
        private async Task CreateTopicIfNotExistsAsync(PublisherServiceApiClient publisher, TopicName topicName)
        {
            try
            {
                await publisher.CreateTopicAsync(topicName);
            }
            catch (RpcException e)
            {
                // If we get any error other than already exists
                if (e.Status.StatusCode != StatusCode.AlreadyExists)
                {
                    throw e;
                }
            }
        }
    }
}
