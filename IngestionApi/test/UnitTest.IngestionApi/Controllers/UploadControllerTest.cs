// ---------------------------------------------------------------------------
// <copyright file="UploadControllerTest.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using CloudProviders;
    using IngestionApp;
    using IngestionApp.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Tests for upload controller.
    /// </summary>
    public class UploadControllerTest
    {
        /// <summary>
        /// Runs through successful upload.
        /// </summary>
        /// <returns>Upload task.</returns>
        [Test]
        public async Task TestUploadSuccess()
        {
            bool shouldSucceed = true;
            var controller = new UploadController(new MockCloudProvider(shouldSucceed), new LoggerFactory().CreateLogger<UploadController>());
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            List<IFormFile> contents = new List<IFormFile>
            {
                this.CreateFile(),
            };

            var result = await controller.Upload(contents);
            var jsonResult = result as JsonResult;
            var response = jsonResult.Value as UploadResponse;

            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Tests failure scenario of upload.
        /// </summary>
        /// <returns>Upload task.</returns>
        [Test]
        public async Task TestUploadFailure()
        {
            bool shouldSucceed = false;

            var controller = new UploadController(new MockCloudProvider(shouldSucceed), new LoggerFactory().CreateLogger<UploadController>());
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            List<IFormFile> contents = new List<IFormFile>
            {
                this.CreateFile(),
            };

            var result = await controller.Upload(contents);
            var jsonResult = result as JsonResult;
            var response = jsonResult.Value as UploadResponse;

            Assert.IsFalse(response.Success);
        }

        /// <summary>
        /// A simple form file.
        /// </summary>
        /// <returns>form file.</returns>
        private FormFile CreateFile()
        {
            return new FormFile(new MemoryStream(Encoding.ASCII.GetBytes("test contents")), 0, 10, "content", "filename");
        }
    }
}