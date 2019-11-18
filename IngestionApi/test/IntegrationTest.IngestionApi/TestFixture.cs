// ---------------------------------------------------------------------------
// <copyright file="TestFixture.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IntegrationTest.IngestionApiTest
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Test Fixture to configure server and client
    /// </summary>
    /// <typeparam name="TStartup">Startup class</typeparam>
    public class TestFixture<TStartup> : IDisposable
    {
        /// <summary>
        /// Test server
        /// </summary>
        private TestServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture{TStartup}"/> class.
        /// </summary>
        public TestFixture()
            : this(Path.Combine(string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture{TStartup}"/> class.
        /// </summary>
        /// <param name="projectParentDir">target project directory</param>
        protected TestFixture(string projectParentDir)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(projectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var webHostBuilder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartup));

            // Create instance of test server
            this.server = new TestServer(webHostBuilder);
            this.server.BaseAddress = new Uri(Constants.ApiUrl);

            // Add configuration for client
            this.Client = this.server.CreateClient();
            this.Client.BaseAddress = new Uri(Constants.ApiUrl);

            // Allow our custom middleware to give us access to api
            this.Client.DefaultRequestHeaders.Add(Constants.HeaderName, "test");
            this.Client.DefaultRequestHeaders.Add(Constants.HeaderId, "12345");
        }

        /// <summary>
        /// Http Client
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Gets absolute path to the project's csproj
        /// </summary>
        /// <param name="projectRelativePath">Starting search path</param>
        /// <param name="startupAssembly">Startup assembly</param>
        /// <returns>Absolute path to project</returns>
        public static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;
            var applicationBasePath = AppContext.BaseDirectory;
            var directoryInfo = new DirectoryInfo(applicationBasePath);

            while (directoryInfo.Parent != null)
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));

                if (projectDirectoryInfo.Exists)
                {
                    if (new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj")).Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        /// <summary>
        /// Disposes disposable properties
        /// </summary>
        public void Dispose()
        {
            this.Client.Dispose();
            this.server.Dispose();
        }
    }
}