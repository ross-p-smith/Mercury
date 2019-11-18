// ---------------------------------------------------------------------------
// <copyright file="DownloadController.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    using CloudProviders;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Download file controller
    /// </summary>
    public class DownloadController : Controller
    {
        /// <summary>
        /// Cloud services provider
        /// </summary>
        private ICloudProvider cloudProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadController"/> class.
        /// </summary>
        /// <param name="cloudProvider">Cloud specific uploader</param>
        public DownloadController(ICloudProvider cloudProvider)
        {
            this.cloudProvider = cloudProvider;
        }

        /// <summary>
        /// Downloads file
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>Downloaded file stream</returns>
        [HttpGet]
        [Route("api/[controller]")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            (var success, var stream) = await this.cloudProvider.StorageProvider().DownloadFile(fileName);
            if (!success)
            {
                return this.NotFound();
            }

            return this.File(stream, "application/octet-stream");
        }
    }
}
