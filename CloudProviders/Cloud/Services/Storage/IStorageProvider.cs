// ---------------------------------------------------------------------------
// <copyright file="IStorageProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Cloud Storage Service Provider Interface
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Asynchronously uploads a file to cloud storage
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="stream">file stream</param>
        /// <returns>a tuple containing success and remote path</returns>
        Task<(bool, string)> UploadFile(string filename, Stream stream = null);

        /// <summary>
        /// Asynchronously downloads a file
        /// </summary>
        /// <param name="filename">filename</param>
        /// <returns>a tuple containing success and content</returns>
        Task<(bool, Stream)> DownloadFile(string filename);
    }
}