// ---------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp
{
    using System;

    /// <summary>
    /// Constants for Ingestion-api
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Key to signal to disable oauth
        /// </summary>
        public const string DisableOauthPermanently = "DISABLE_OAUTH_PERMANENTLY";

        /// <summary>
        /// Value to signal to disable oauth
        /// </summary>
        public const string DisableOauthForCI = "DISABLE_OAUTH_FOR_CI";
    }
}
