// ---------------------------------------------------------------------------
// <copyright file="DownloadControllerTest.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using CloudProviders;
    using IngestionApp.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    /// <summary>
    /// Download controller tests
    /// </summary>
    public class DownloadControllerTest
    {
        /// <summary>
        /// Successful test run of download controller
        /// </summary>
        /// <returns>Download Task</returns>
        [Test]
        public async Task DownloadSuccess()
        {
            bool shouldSucceed = true;
            string filename = "SuccessDownload.txt";
            string content = "Successful downloaded content";

            // Setup cloud provider with a file
            ICloudProvider cloudProvider = new MockCloudProvider(shouldSucceed);
            this.UploadFile(filename, content, cloudProvider);

            var controller = new DownloadController(cloudProvider);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.DownloadFile(filename);
            Assert.That(result, Is.Not.EqualTo(null));

            // verify the contents of the file stream
            Assert.That(result, Is.InstanceOf(typeof(FileStreamResult)));
            FileStreamResult fileStreamResult = result as FileStreamResult;

            Assert.That(fileStreamResult.FileStream, Is.Not.EqualTo(null));
            string fileString = new StreamReader(fileStreamResult.FileStream).ReadToEnd();
            Assert.That(fileString, Is.EqualTo(content));
        }

        /// <summary>
        /// Successful test run of download controller
        /// </summary>
        /// <returns>Download Task</returns>
        [Test]
        public async Task DownloadFailure()
        {
            bool shouldSucceed = false;
            var controller = new DownloadController(new MockCloudProvider(shouldSucceed));
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = await controller.DownloadFile("testfile.txt");

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(result, Is.InstanceOf(typeof(NotFoundResult)));
        }

        /// <summary>
        /// Uploads a file to cloud provider
        /// </summary>
        /// <param name="filename">file name</param>
        /// <param name="content">file content</param>
        /// <param name="cloudProvider">cloud provider</param>
        private void UploadFile(string filename, string content, ICloudProvider cloudProvider)
        {
            cloudProvider.StorageProvider().UploadFile(filename, new MemoryStream(Encoding.ASCII.GetBytes(content)));
        }
    }
}
