// ---------------------------------------------------------------------------
// <copyright file="IndexController.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller for index page
    /// </summary>
    [Route("{*url}", Order = 999)]
    public class IndexController : Controller
    {
        /// <summary>
        /// Hosting environment
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexController"/> class.
        /// </summary>
        /// <param name="environment">Host environment</param>
        public IndexController(IHostingEnvironment environment)
        {
            this.hostingEnvironment = environment;
        }

        /// <summary>
        /// Gets index page
        /// </summary>
        /// <returns>index html</returns>
        [HttpGet]
        public PhysicalFileResult Get()
        {
            var file = Path.Combine(this.hostingEnvironment.WebRootPath, "index.html");
            return this.PhysicalFile(file, "text/html");
        }
    }
}
