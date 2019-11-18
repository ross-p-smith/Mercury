// ---------------------------------------------------------------------------
// <copyright file="CloudErrorException.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;

    /// <summary>
    /// Exception for cloud operations
    /// </summary>
    public class CloudErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudErrorException"/> class.
        /// </summary>
        public CloudErrorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudErrorException"/> class.
        /// </summary>
        /// <param name="error">The error message to be logged </param>
        public CloudErrorException(string error)
            : base(error)
        {
        }
    }
}
