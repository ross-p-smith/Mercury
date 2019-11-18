// ---------------------------------------------------------------------------
// <copyright file="AzureAuthMiddleware.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Azure Cloud oAuth middleware interface
    /// </summary>
    public class AzureAuthMiddleware : AuthMiddlewareCommon, IAuthMiddleware
    {
        /// <summary>
        /// Azure configuration
        /// </summary>
        private AzureConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureAuthMiddleware" /> class
        /// </summary>
        /// <param name="cloud">Azure config</param>
        public AzureAuthMiddleware(AzureConfiguration cloud)
        {
            this.cloud = cloud;
        }

        /// <summary>
        /// Adds Azure oAuth authorization to the service collection
        /// </summary>
        /// <param name="services">The service collection to add Azure oAuth authorization to</param>
        /// <returns>cloud storage provider</returns>
        public IServiceCollection AddCloudAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options =>
                {
                    options.ClientId = this.cloud.AzureAD.ClientId;
                    options.TenantId = this.cloud.AzureAD.TenantId;
                    options.Domain = this.cloud.AzureAD.Domain;
                    options.CallbackPath = this.cloud.AzureAD.CallbackPath;
                    options.Instance = this.cloud.AzureAD.Instance;
                });

            services.Configure<OpenIdConnectOptions>(
                AzureADDefaults.OpenIdScheme,
                options =>
                {
                    options.Authority = options.Authority + "/v2.0/";         // Microsoft identity platform
                    options.TokenValidationParameters.ValidateIssuer = false; // accept several tenants (here simplified)
                });

            return services;
        }
    }
}