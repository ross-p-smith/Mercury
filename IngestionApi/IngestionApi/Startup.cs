// ---------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IngestionApp
{
    using System;
    using CloudProviders;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Application startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The App Insights Logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Cloud Provider
        /// </summary>
        private ICloudProvider cloudProvider;

        /// <summary>
        /// The Current Application Environment
        /// </summary>
        private IHostingEnvironment currentEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">App configuration</param>
        /// <param name="env">Hosting environment</param>
        /// <param name="logger">Logger used for tracing</param>
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false)
                            .AddJsonFile("secrets.json", optional: true)
                            .AddJsonFile("/flexvol/secrets.json", optional: true)
                            .AddEnvironmentVariables();

            this.Configuration = builder.Build();
            this.logger = logger;
            CloudLogger.SetLogger(logger);
            this.cloudProvider = new CloudFactory(this.Configuration).Provider();
            this.currentEnvironment = env;
        }

        /// <summary>
        /// Gets app configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Method called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            if (string.IsNullOrEmpty(this.Configuration.GetSection("ApplicationInsights")?.GetValue<string>("InstrumentationKey")))
            {
                this.logger.LogWarning("Application insights has been disabled");
            }
            else
            {
                // App will crash if application insights is created without an instrumentation key
                services.AddApplicationInsightsTelemetry();
                if (this.currentEnvironment.IsDevelopment())
                {
                    this.logger.LogWarning("Service Profiler has been Disabled");
                }
                else
                {
                    // NOTE: services.AddServiceProfiler() will not work on Mac as it is in beta
                    // Profiler should be added first. Make sure that log level is debug or higher in appsettings.json
                    services.AddServiceProfiler();
                }
            }

            // Change as of .NET 2.1 requiring explicit port assingment https://github.com/aspnet/AspNetCore/issues/3176
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            // Ingress Header Forwarding
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddLogging();
            services.AddSingleton<ICloudProvider>(s => this.cloudProvider);
            var middleware = this.cloudProvider.AuthMiddleware();
            services = middleware.AddAuthorization(services);

            if (this.ShouldSkipOauth())
            {
                services.AddMvc(opts =>
                {
                    opts.Filters.Add(new AllowAnonymousFilter());
                });
                return;
            }

            services = middleware.AddCloudAuthorization(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Host environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Use headers forwarded by ingress
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            this.cloudProvider.AuthMiddleware().ConfigureApp(app, env);

            // This must come before app.UseMvc() for testing
            this.ConfigureAdditionalMiddleware(app);
            app.UseMvc();
        }

        /// <summary>
        /// Configures additional middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        protected virtual void ConfigureAdditionalMiddleware(IApplicationBuilder app)
        {
        }

        /// <summary>
        /// Should OAuth be added to services
        /// </summary>
        /// <returns>If should skip oauth</returns>
        private bool ShouldSkipOauth()
        {
            var shouldDisable = this.Configuration.GetValue<string>(Constants.DisableOauthPermanently);
            if (shouldDisable != null)
            {
                return shouldDisable == Constants.DisableOauthForCI;
            }

            return false;
        }
    }
}