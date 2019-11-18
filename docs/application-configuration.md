# Configuring Managed Identities
## Azure
See here for [Azure](./managed-identities.md#azure-managed-service-identities)
## GCP
See here for [GCP](./managed-identities.md#google-service-account)

# Application Configuration

For deployment flexibility configuration is loaded at runtime from any of the 
following locations in the following precedence:

1. Environment Variables
2. Application Environment JSON file
3. Application JSON file
4. An optional secrets JSON file
5. An optional JSON file that could be stored on a flexvolume

_Config names are case-insensitive and later configuration sources overwrite 
earlier ones_

Due to inherent differences in cloud providers the supplied configuration will 
change. For any cloud provider there will be general config and cloud specific 
config.


## General Application Configuration Example

As **appsettings.json:**

```json
{
  "StorageFolder": "workfiles",
  "QueueName": "workqueue"
}
```
As **Environment Variables:**

### Bash

```sh
$ export StorageFolder="workfiles"
$ export QueueName="workqueue"
```

### Powershell

```powershell
> $env:StorageFolder="workfiles"
> $env:QueueName="workqueue"
```

## Azure Configuration Example

As **appsettings.json:**

```json
{
  "cloud": "azure",
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "localhost",
    "TenantId": "",
    "ClientId": "",
    "CallbackPath": "/signin-oidc"
  },
  "AzureStorageAcountName" : ""
}
```

As **Environment Variables:**

### Bash
```sh
$ export cloud="azure"
$ export AzureAd__Insance="https://login.microsoftonline.com/"
$ export AzureAd__Domain="localhost"
$ export AzureAd__TenantId=""
$ export AzureAd__ClientId=""
$ export AzureAd__CallbackPath="/signin-oidc"
$ export AzureStorageAccountName=""
```

### Powershell
```powershell
> $env:cloud="azure"
> $env:AzureAd__Insance="https://login.microsoftonline.com/"
> $env:AzureAd__Domain="localhost"
> $env:AzureAd__TenantId=""
> $env:AzureAd__ClientId=""
> $env:AzureAd__CallbackPath="/signin-oidc"
> $env:AzureStorageAccountName=""
```

## GCP Configuration Example

As **appsettings.json:**
```json
{
  "cloud": "gcp",
  "ProjectID": "",
  "GoogleAuthenticationClientId": "",
  "GoogleAuthenticationClientSecret": ""
}
```

As **Environment Variables:**

### Bash
```sh
$ export cloud="gcp"
$ export ProjectID=""
$ export GoogleAuthenticationClientId=""
$ export GoogleAuthenticationClientSecret=""
```

### Powershell

```powershell
> $env:cloud="gcp"
> $env:ProjectID=""
> $env:GoogleAuthenticationClientId=""
> $env:GoogleAuthenticationClientSecret=""
```


