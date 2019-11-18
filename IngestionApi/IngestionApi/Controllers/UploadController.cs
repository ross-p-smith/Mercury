// ---------------------------------------------------------------------------
// <copyright file="UploadController.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using CloudProviders;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Controller for FileForm upload
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        /// <summary>
        /// Cloud services provider
        /// </summary>
        private readonly ICloudProvider cloudProvider;

        /// <summary>
        ///  App Insights Logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="cloudProvider">Cloud specific uploader</param>
        /// <param name="logger">Application Insights logger</param>
        public UploadController(ICloudProvider cloudProvider, ILogger<UploadController> logger)
        {
            this.cloudProvider = cloudProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Uploads files
        /// </summary>
        /// <param name="files">files to upload</param>
        /// <returns>Upload success</returns>
        [HttpPost]
        [HttpPut]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var response = new UploadResponse { Success = true };
            this.Response.StatusCode = (int)HttpStatusCode.OK;

            string operationId = System.Diagnostics.Activity.Current?.RootId ?? "Could not pull Operation Id";

            foreach (var formFile in files)
            {
                var uploadSuccess = false;

                if (formFile.Length > 0)
                {
                    string remoteFilePath = null;

                    using (var stream = formFile.OpenReadStream())
                    {
                        (uploadSuccess, remoteFilePath) = await this.cloudProvider.StorageProvider().UploadFile(formFile.FileName, stream);

                        var metadata = new QueueMetadata { Fileuri = remoteFilePath, OperationId = operationId };

                        uploadSuccess = uploadSuccess && await this.cloudProvider.QueueProvider().Enqueue(metadata);
                    }
                }

                this.LogFileUploadStatistics(formFile, operationId, uploadSuccess);

                if (uploadSuccess == false)
                {
                    response.Success = false;
                    break;
                }
            }

            if (response.Success == false)
            {
                this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return this.Json(response);
        }

        /// <summary>
        /// Logs information about the file upload to Application Insights
        /// </summary>
        /// <param name="file">File that was uploaded </param>
        /// <param name="operationId">Operation ID used to show End-To-End processing of files</param>
        /// <param name="uploadSuccess">Whether the file was successfully uploaded</param>
        private void LogFileUploadStatistics(IFormFile file, string operationId, bool uploadSuccess)
        {
            using (this.logger.BeginScope(new Dictionary<string, object>
                {
                    { "File", $"Name: {file.FileName}, Size: {file.Length}" },
                    { "OperationId", operationId },
                }))
            {
                if (uploadSuccess == false)
                {
                    this.logger.LogError("File failed to upload.");
                }
                else
                {
                    this.logger.LogInformation("File successfully uploaded.");
                }
            }
        }
    }
}
