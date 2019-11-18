// ---------------------------------------------------------------------------
// <copyright file="GoogleStorageProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Google.Cloud.Storage.V1;

    /// <summary>
    /// Google cloud storage service provider
    /// </summary>
    public class GoogleStorageProvider : IStorageProvider
    {
        /// <summary>
        /// Google cloud configuration
        /// </summary>
        private GoogleConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleStorageProvider" /> class
        /// </summary>
        /// <param name="cloud">Google cloud config</param>
        public GoogleStorageProvider(GoogleConfiguration cloud)
        {
            this.cloud = cloud;
        }

        /// <summary>
        /// Asynchronously uploads a file to Google cloud storage
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="stream">file stream</param>
        /// <returns>a tuple containing success and remote path</returns>
        public async Task<(bool, string)> UploadFile(string filename, Stream stream = null)
        {
            var storage = StorageClient.Create();

            try
            {
                await storage.UploadObjectAsync(this.cloud.StorageFolder, filename, null, stream);
            }
            catch (Exception)
            {
                // TODO: Logging
                return (false, null);
            }

            return (true, $"{this.cloud.StorageFolder}/{filename}");
        }

        /// <summary>
        /// Downloads a file
        /// </summary>
        /// <param name="filename">file name</param>
        /// <returns>success and content</returns>
        public async Task<(bool, Stream)> DownloadFile(string filename)
        {
            var storage = StorageClient.Create();
            Stream stream = new MemoryStream();
            try
            {
                await storage.DownloadObjectAsync(this.cloud.StorageFolder, filename, stream);
                stream.Position = 0;
                return (true, stream);
            }
            catch
            {
                // TODO: Logging
                return (false, null);
            }
        }
    }
}
