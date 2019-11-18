// ---------------------------------------------------------------------------
// <copyright file="GoogleConfiguration.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;

    /// <summary>
    /// Google Cloud Configuration
    /// </summary>
    public class GoogleConfiguration : CloudConfiguration
    {
        /// <summary>
        /// Gets or sets Google Cloud project id
        /// </summary>
        public string GoogleProjectID { get; set; }

        /// <summary>
        /// Gets or sets Google Cloud oAuth client id
        /// </summary>
        public string GoogleAuthenticationClientId { get; set; }

        /// <summary>
        /// Gets or sets Google Cloud oAuth client secret
        /// </summary>
        public string GoogleAuthenticationClientSecret { get; set; }

        /// <summary>
        /// Gets or sets Google Subscription Name
        /// </summary>
        public string GoogleSubscriptionName { get; set; }

        /// <summary>
        /// Logs configuration
        /// </summary>
        public new void LogConfiguration()
        {
            Console.WriteLine("ProjectID={0}", this.GoogleProjectID);
            base.LogConfiguration();
        }
    }
}
