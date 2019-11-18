// ---------------------------------------------------------------------------
// <copyright file="TestAuthenticatedMiddleware.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace IntegrationTest.IngestionApiTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Middleware for allowing tests to be pre authorized
    /// </summary>
    public class TestAuthenticatedMiddleware
    {
        /// <summary>
        /// Cookie authentication
        /// </summary>
        public const string TestingCookieAuthentication = "TestCookieAuthentication";

        /// <summary>
        /// Request delegate
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAuthenticatedMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request delegate</param>
        public TestAuthenticatedMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Grants user a new claim prinipal if headers contain correct name and id
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Task awaiting request delegate</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(Constants.HeaderName))
            {
                var name = context.Request.Headers[Constants.HeaderName].First();
                string id = string.Empty;
                if (context.Request.Headers.Keys.Contains(Constants.HeaderId))
                {
                    id = context.Request.Headers[Constants.HeaderId].First();
                }

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.NameIdentifier, id),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, TestingCookieAuthentication);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
            }

            await this.next(context);
        }
    }
}
