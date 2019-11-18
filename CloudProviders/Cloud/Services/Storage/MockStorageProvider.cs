// ---------------------------------------------------------------------------
// <copyright file="MockStorageProvider.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Mock storage provider
    /// </summary>
#pragma warning disable 1998
    public class MockStorageProvider : IStorageProvider
    {
        /// <summary>
        /// flag to tell if upload will succeed.
        /// </summary>
        private bool shouldSucceed;

        /// <summary>
        /// Mock storage
        /// </summary>
        private Dictionary<string, string> storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockStorageProvider"/> class.
        /// </summary>
        /// <param name="shouldSucceed">Whether upload should succeed.</param>
        public MockStorageProvider(bool shouldSucceed)
        {
            this.shouldSucceed = shouldSucceed;
            this.storage = new Dictionary<string, string>();
        }

        /// <summary>
        /// Downloads file
        /// </summary>
        /// <param name="filename">file name</param>
        /// <returns>success and the stream</returns>
        public async Task<(bool, Stream)> DownloadFile(string filename)
        {
            if (this.shouldSucceed)
            {
                string content;
                if (this.storage.TryGetValue(filename, out content))
                {
                    var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
                    return (true, stream);
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Mocks uploading a file to cloud storage
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="stream">file stream</param>
        /// <returns>a tuple containing success and a very fake remote path</returns>
        public async Task<(bool, string)> UploadFile(string filename, Stream stream = null)
        {
            if (this.shouldSucceed)
            {
                if (stream != null)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        this.storage.Add(filename, sr.ReadToEnd());
                    }
                }
            }

            return (this.shouldSucceed, filename);
        }
    }
}
