// ---------------------------------------------------------------------------
// <copyright file="UploadTest.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IntegrationTest.IngestionApiTest
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using CloudProviders;
    using IntegrationTest.IngestionApiTest;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// Integration tests for upload
    /// </summary>
    public class UploadTest : IClassFixture<TestFixture<TestStartup>>
    {
        /// <summary>
        /// Http Client
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// Key for environment variable setting of external server address
        /// </summary>
        private string externalClientFlag = "API_BASE_ADDRESS";

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadTest"/> class.
        /// </summary>
        /// <param name="fixture">Fixture containing startup</param>
        public UploadTest(TestFixture<TestStartup> fixture)
        {
            this.client = this.GetClient(fixture);
        }

        /// <summary>
        /// Upload/Download test. First verify file hasn't been uploaded, then
        /// upload, finally verify file exists and its content is correct
        /// </summary>
        /// <returns>Task from POST</returns>
        [Fact]
        public async Task UploadDownload()
        {
            // Ensure that storage and queue state is not initialized for no-op success
            string fileDownloadRequest = string.Format("{0}/?={1}", Constants.DownloadRoute, Constants.Filename);
            var downloadResponse = await this.client.GetAsync(fileDownloadRequest);

            Console.WriteLine("Checking if storage is empty {0}", fileDownloadRequest);
            Assert.Equal(HttpStatusCode.NotFound, downloadResponse.StatusCode);
            Assert.False(await this.MessageIsProcessedInQueueAsync(1));

            Console.WriteLine("Storage is ready for upload");
            var file = this.GetFileStreamContent(Constants.Filename);
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(file, Constants.UploadControllerFileParameter, Constants.Filename);
                Console.WriteLine("Uploading file");
                var response = await this.client.PostAsync(Constants.UploadRoute, formData);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            Console.WriteLine("Verifying upload success");
            downloadResponse = await this.client.GetAsync(fileDownloadRequest);

            Assert.Equal(HttpStatusCode.OK, downloadResponse.StatusCode);
            string actual = await this.ExtractResponseContent(downloadResponse);
            string expected = this.ToString(this.GetFileStream(Constants.Filename));
            Assert.Equal(expected, actual);
            Console.WriteLine("Upload successful");
            Console.WriteLine("Verfying queue message is processed");

            // Assertions that depend on message processor
            Assert.True(await this.MessageIsProcessedInQueueAsync(65));
            Console.WriteLine("Queue message has been processed");
        }

        /// <summary>
        /// Assertions in this method are based on MessageProcessor which picks
        /// up an item from the queue and writes a file back to storage
        /// </summary>
        /// <param name="times">number of times to retry</param>
        /// <returns>If queue item was processed</returns>
        private async Task<bool> MessageIsProcessedInQueueAsync(int times)
        {
            HttpResponseMessage response;
            int attempts;

            (response, attempts) = await this.Retry(times, new TimeSpan(5 * TimeSpan.TicksPerSecond), this.DownloadQueueResponseAsync);
            if (response == null)
            {
                return false;
            }

            var actual = await this.ExtractResponseContent(response);
            QueueMetadata data = JsonConvert.DeserializeObject<QueueMetadata>(actual);
            if (data == null)
            {
                return false;
            }

            Console.WriteLine("Queue message proccessed after {0} retries", attempts);
            return !string.IsNullOrEmpty(data.Fileuri);
        }

        /// <summary>
        /// Downloads the notification file from MessageProcessor
        /// </summary>
        /// <returns>http response</returns>
        private async Task<HttpResponseMessage> DownloadQueueResponseAsync()
        {
            string route = string.Format("{0}?={1}", Constants.DownloadRoute, Constants.NotifyFilename);
            return await this.client.GetAsync(route);
        }

        /// <summary>
        /// Retries http request
        /// </summary>
        /// <param name="times">number of times to try</param>
        /// <param name="delay">delay between tries</param>
        /// <param name="operation">operation to try</param>
        /// <returns>http response message</returns>
        private async Task<(HttpResponseMessage, int)> Retry(int times, TimeSpan delay, Func<Task<HttpResponseMessage>> operation)
        {
            int attempts = 0;
            HttpResponseMessage messageResponse;
            while (true)
            {
                try
                {
                    attempts++;
                    messageResponse = await operation();
                    if (!messageResponse.IsSuccessStatusCode)
                    {
                        throw new Exception(string.Format("{0}", messageResponse.ReasonPhrase));
                    }

                    break;
                }
                catch
                {
                    if (attempts == times)
                    {
                        return (null, attempts);
                    }

                    Task.Delay(delay).Wait();
                }
            }

            return (messageResponse, attempts);
        }

        /// <summary>
        /// Get file stream
        /// </summary>
        /// <param name="filename">filename</param>
        /// <returns>File Stream</returns>
        private FileStream GetFileStream(string filename)
        {
            return File.OpenRead(Path.Combine(Constants.FileContentPath, filename));
        }

        /// <summary>
        /// Get Stream content of file
        /// </summary>
        /// <param name="filename">file name</param>
        /// <returns>Stream content of file</returns>
        private StreamContent GetFileStreamContent(string filename)
        {
            return new StreamContent(this.GetFileStream(filename));
        }

        /// <summary>
        /// Converts stream to string
        /// </summary>
        /// <param name="stream">stream content</param>
        /// <returns>string content</returns>
        private string ToString(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Gets content from http download response
        /// </summary>
        /// <param name="downloadResponse">http download response</param>
        /// <returns>string content</returns>
        private async Task<string> ExtractResponseContent(HttpResponseMessage downloadResponse)
        {
            using (var result = await downloadResponse.Content.ReadAsStreamAsync())
            {
                using (var rres = new GZipStream(result, CompressionMode.Decompress))
                {
                    using (var sr = new StreamReader(result))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Gets client based on prescense of environment variable
        /// </summary>
        /// <param name="fixture">Test framework app</param>
        /// <returns>Http client</returns>
        private HttpClient GetClient(TestFixture<TestStartup> fixture)
        {
            var baseAddr = Environment.GetEnvironmentVariable(this.externalClientFlag);
            HttpClient httpClient;
            if (string.IsNullOrEmpty(baseAddr))
            {
                httpClient = fixture.Client;
            }
            else
            {
               httpClient = new HttpClient() { BaseAddress = this.GetUri(baseAddr) };
               Console.WriteLine("Aiming at an external endpoint with different configurations");
            }

            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
            };

            Console.WriteLine(string.Format("IntegrationTests running against client address {0}", httpClient.BaseAddress));
            return httpClient;
        }

        /// <summary>
        /// Gets uri from string
        /// </summary>
        /// <param name="s">string base address</param>
        /// <returns>Uri for s</returns>
        private Uri GetUri(string s)
        {
            UriBuilder ub = new UriBuilder(s)
            {
                Scheme = Uri.UriSchemeHttp,
                Port = -1,
            };

            return ub.Uri;
        }
    }
}