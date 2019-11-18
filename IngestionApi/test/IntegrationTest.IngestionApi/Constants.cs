// ---------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IntegrationTest.IngestionApiTest
{
    /// <summary>
    /// Class to maintain constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Test Header name
        /// </summary>
        public const string HeaderName = "testname";

        /// <summary>
        /// Test Header id
        /// </summary>
        public const string HeaderId = "testid";

        /// <summary>
        /// Parameter name for the file upload api
        /// </summary>
        public const string UploadControllerFileParameter = "files";

        /// <summary>
        /// File name
        /// </summary>
        public const string Filename = "file.txt";

        /// <summary>
        /// Relative path to test files
        /// </summary>
        public const string FileContentPath = "Resources/";

        /// <summary>
        /// Route to upload api
        /// </summary>
        public const string UploadRoute = "/api/upload";

        /// <summary>
        /// Api Url
        /// </summary>
        public const string ApiUrl = "https://localhost:5000";

        /// <summary>
        /// Route to download api
        /// </summary>
        public const string DownloadRoute = "/api/download";

        /// <summary>
        /// Filename the processor will write to
        /// </summary>
        public const string NotifyFilename = "NotifyReceived.txt";
    }
}
