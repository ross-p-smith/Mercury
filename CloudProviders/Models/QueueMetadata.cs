// ---------------------------------------------------------------------------
// <copyright file="QueueMetadata.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    /// <summary>
    /// Metadata stored in queue
    /// </summary>
    public class QueueMetadata
    {
        /// <summary>
        /// Gets or sets Fileuri
        /// </summary>
        public string Fileuri { get; set; }

        /// <summary>
        /// Gets or sets the Operation ID used for Application Insights
        /// </summary>
        public string OperationId { get; set; }
    }
}
