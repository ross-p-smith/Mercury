// ---------------------------------------------------------------------------
// <copyright file="TestStartup.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IntegrationTest.IngestionApiTest
{
    using System;
    using global::IngestionApp;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Startup class used for testing only
    /// </summary>
    public class TestStartup : Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStartup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration properties</param>
        /// <param name="env">Hosting environment</param>
        public TestStartup(IConfiguration configuration, IHostingEnvironment env)
            : base(configuration, env, new LoggerFactory().CreateLogger<Startup>())
        {
        }

        /// <summary>
        /// Adds middleware to authenticate tests
        /// </summary>
        /// <param name="app">Application builder</param>
        protected override void ConfigureAdditionalMiddleware(IApplicationBuilder app)
        {
            app.UseMiddleware<TestAuthenticatedMiddleware>();
        }
    }
}