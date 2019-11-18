// ---------------------------------------------------------------------------
// <copyright file="IAuthMiddleware.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Cloud oAuth middleware interface
    /// </summary>
    public interface IAuthMiddleware
    {
        /// <summary>
        /// Adds oauth authorization to the service collection
        /// </summary>
        /// <param name="services">The service collection to add oAuth authorization to</param>
        /// <returns>cloud storage provider</returns>
        IServiceCollection AddCloudAuthorization(IServiceCollection services);

        /// <summary>
        /// Adds oauth authorization to the service
        /// </summary>
        /// <param name="services">The service to add oauth authorization to </param>
        /// <returns>Returns the services object with new ouath authorization, this can be used to chain steps together</returns>
        IServiceCollection AddAuthorization(IServiceCollection services);

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        void ConfigureApp(IApplicationBuilder app, IHostingEnvironment env);
    }
}