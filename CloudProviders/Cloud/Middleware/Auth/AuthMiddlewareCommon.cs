// ---------------------------------------------------------------------------
// <copyright file="AuthMiddlewareCommon.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Cloud oAuth middleware interface
    /// </summary>
    public abstract class AuthMiddlewareCommon
    {
        /// <summary>
        /// Adds oauth authorization to the service
        /// </summary>
        /// <param name="services">The service to add oauth authorization to </param>
        /// <returns>Returns the services object with new ouath authorization, this can be used to chain steps together</returns>
        public IServiceCollection AddAuthorization(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy(
                    "AuthenticatedUser",
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return services;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        public void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
        }
    }
}