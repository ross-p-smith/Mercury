// ---------------------------------------------------------------------------
// <copyright file="AzureStorageProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    /// Azure storage service provider
    /// </summary>
    public class AzureStorageProvider : IStorageProvider
    {
        /// <summary>
        /// Azure Blob Storage Container Object
        /// </summary>
        private CloudBlobContainer cloudBlobContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageProvider" /> class
        /// </summary>
        /// <param name="cloud">Azure config</param>
        public AzureStorageProvider(AzureConfiguration cloud)
        {
            var storageAccount = cloud.GetStorageAccountAsync().Result;
            if (storageAccount == null)
            {
                throw new CloudErrorException();
            }

            this.cloudBlobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference(cloud.StorageFolder);
            this.cloudBlobContainer.CreateIfNotExists();
        }

        /// <summary>
        /// Downloads a file
        /// </summary>
        /// <param name="filename">file name</param>
        /// <returns>success and content</returns>
        public async Task<(bool, Stream)> DownloadFile(string filename)
        {
            MemoryStream stream = new MemoryStream();

            var blob = this.cloudBlobContainer.GetBlobReference(filename);
            if (blob.Exists())
            {
                await blob.DownloadToStreamAsync(stream);
                stream.Position = 0;
                return (true, stream);
            }

            return (false, null);
        }

        /// <summary>
        /// Asynchronously uploads a file to Azure storage
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="stream">file stream</param>
        /// <returns>a tuple containing success and remote path</returns>
        public async Task<(bool, string)> UploadFile(string filename, Stream stream = null)
        {
            try
            {
                // Get a reference to the blob address
                CloudBlockBlob cloudBlockBlob = this.cloudBlobContainer.GetBlockBlobReference(filename);

                // Upload the file to the blob
                if (stream == null)
                {
                    return (false, null);
                }

                await cloudBlockBlob.UploadFromStreamAsync(stream);

                return (true, cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
            }
            catch (StorageException)
            {
                return (false, null);
            }
        }
    }
}
