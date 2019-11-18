// ---------------------------------------------------------------------------
// <copyright file="AzureConfiguration.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace CloudProviders
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;

    /// <summary>
    /// Azure Cloud Configuration
    /// </summary>
    public class AzureConfiguration : CloudConfiguration
    {
        /// <summary>
        /// Storage resource endpoint in Azure
        /// </summary>
        private readonly string azureStorageResourceEndpoint = "https://storage.azure.com/";

        /// <summary>
        /// Storage account suffix
        /// </summary>
        private readonly string azureStorageAccountEndpointSuffix = "core.windows.net";

        /// <summary>
        /// Gets or sets the Storage Account name in Azure
        /// </summary>
        public string AzureStorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets AzureAD config settings
        /// </summary>
        public AzureADOptions AzureAD { get; set; }

        /// <summary>
        /// Access token for storage
        /// </summary>
        public string AzureStorageAccessToken { get; set; }

        /// <summary>
        /// Gets the Azure storage account asynchronously using Manage Service Identities (MSI)
        /// </summary>
        /// <returns>azure storage account</returns>
        public async Task<CloudStorageAccount> GetStorageAccountAsync()
        {
            AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();
            var accessToken = this.AzureStorageAccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await tokenProvider.GetAccessTokenAsync(this.azureStorageResourceEndpoint, this.AzureAD.TenantId);
            }

            StorageCredentials storageCredentials = new StorageCredentials(new TokenCredential(accessToken));
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, this.AzureStorageAccountName, this.azureStorageAccountEndpointSuffix, true);
            return cloudStorageAccount;
        }

        /// <summary>
        /// Logs configuration
        /// </summary>
        public new void LogConfiguration()
        {
            Console.WriteLine("AzureStorageAccountName={0}", this.AzureStorageAccountName);
            Console.WriteLine("AzureAD__ClientID={0}", this.AzureAD.ClientId);
            Console.WriteLine("AzureAD__TenantID={0}", this.AzureAD.TenantId);
            base.LogConfiguration();
        }
    }
}
