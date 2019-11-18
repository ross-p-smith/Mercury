// ---------------------------------------------------------------------------
// <copyright file="UploadResponse.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Upload response
    /// </summary>
    public class UploadResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the upload was successful
        /// </summary>
        public bool Success { get; set; }
    }
}