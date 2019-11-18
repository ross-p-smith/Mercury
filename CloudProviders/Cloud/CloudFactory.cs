// ---------------------------------------------------------------------------
// <copyright file="CloudFactory.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Initializes the appropriate cloud configuration and provider
    /// </summary>
    public class CloudFactory
    {
        /// <summary>
        /// Application configuration
        /// </summary>
        private IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFactory" /> class
        /// </summary>
        /// <param name="config">Application config</param>
        public CloudFactory(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets the appropriate cloud provider
        /// </summary>
        /// <returns>cloud provider</returns>
        public ICloudProvider Provider()
        {
            // Our cloud provider selection will be based on a config value "cloud"
            // that can be set in any of the configuration sources
            string cloudProvider = this.config.GetValue<string>("cloud");

            /*
             * If we have a known cloud provider then we will instantiate the configuration and
             * attempt to bind the config object to configuration values by matching
             * property names against configuration keys recursively
             * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2
             */

            if (cloudProvider == "azure")
            {
                var cloudConfig = new AzureConfiguration();
                this.config.Bind(cloudConfig);
                return new AzureProvider(cloudConfig);
            }
            else if (cloudProvider == "gcp")
            {
                var cloudConfig = new GoogleConfiguration();
                this.config.Bind(cloudConfig);
                return new GoogleProvider(cloudConfig);
            }
            else if (cloudProvider == "development")
            {
                // Initialize the MockCloudProvider in development to facilitate api logic testing
                return new MockCloudProvider(true);
            }

            throw new ArgumentOutOfRangeException("cloud", "Empty or invalid");
        }
    }
}
