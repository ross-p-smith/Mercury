// ---------------------------------------------------------------------------
// <copyright file="GoogleAuthMiddleware.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Google Cloud oAuth middleware interface
    /// </summary>
    public class GoogleAuthMiddleware : AuthMiddlewareCommon, IAuthMiddleware
    {
        /// <summary>
        /// Google configuration
        /// </summary>
        private GoogleConfiguration cloud;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAuthMiddleware" /> class
        /// </summary>
        /// <param name="cloud">Google config</param>
        public GoogleAuthMiddleware(GoogleConfiguration cloud)
        {
            this.cloud = cloud;
        }

        /// <summary>
        /// Adds Google oAuth authorization to the service collection
        /// </summary>
        /// <param name="services">The service collection to add Google oAuth authorization to</param>
        /// <returns>cloud storage provider</returns>
        public IServiceCollection AddCloudAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(v =>
            {
                v.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                v.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                v.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = this.cloud.GoogleAuthenticationClientId;
                options.ClientSecret = this.cloud.GoogleAuthenticationClientSecret;
            });
            return services;
        }
    }
}