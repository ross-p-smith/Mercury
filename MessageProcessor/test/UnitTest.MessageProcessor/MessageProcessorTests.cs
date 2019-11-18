// ---------------------------------------------------------------------------
// <copyright file="MessageProcessorTests.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace Tests
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using CloudProviders;
    using MessageProcessor;
    using Newtonsoft.Json;
    using NUnit.Framework;

    /// <summary>
    /// Message processor tests
    /// </summary>
    public class MessageProcessorTests
    {
        /// <summary>
        /// Cloud provider
        /// </summary>
        private ICloudProvider cloudProvider;

        /// <summary>
        /// Initialize cloud provider
        /// </summary>
        [SetUp]
        public void Init()
        {
            this.cloudProvider = new MockCloudProvider(true);
        }

        /// <summary>
        /// Start MessageProcessor, enqueue an item and watch notification handler
        /// process the item
        /// </summary>
        /// <returns>Task from file operations</returns>
        [Test]
        public async Task MessageProcessorTest()
        {
            (bool success, Stream content) = await this.cloudProvider.StorageProvider().DownloadFile(Constants.NotifyFilename);
            Assert.That(success, Is.False);
            Assert.That(content, Is.Null);
            string expectedUri = "https://contenturi";
            _ = await this.cloudProvider.QueueProvider().Enqueue(new QueueMetadata() { Fileuri = expectedUri });

            QueueWatcher watcher = new QueueWatcher(this.cloudProvider, 0);
            NotificationHandler handler = new NotificationHandler(this.cloudProvider);
            MessageProcessor.Run(watcher, handler).Wait();

            (success, content) = await this.cloudProvider.StorageProvider().DownloadFile(Constants.NotifyFilename);

            Assert.That(success, Is.True);
            string stringContent = this.StreamToString(content);
            var actualMetadata = JsonConvert.DeserializeObject<QueueMetadata>(stringContent);
            Assert.That(actualMetadata.Fileuri, Is.EqualTo(expectedUri));
        }

        /// <summary>
        /// Converts Stream to string
        /// </summary>
        /// <param name="stream">Stream to convert</param>
        /// <returns>string version of stream</returns>
        private string StreamToString(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            using (StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
