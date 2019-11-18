# Development Mode
```bash
docker-compose up
```

In a browser, navigate to [http://localhost:5000/](http://localhost:5000/).

This will start the API and MessageProcessor in "Development" mode which will cause each to use its own in-memory queue and storage provider. This will not test the integration between the two services.

# Azure Mode

## Services
* [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/)
* [Azure Storage Queues](https://azure.microsoft.com/en-us/services/storage/queues/)

## Steps

1. Create a new Azure storage account
2. Create a storage blob named `work`
3. Create a storage queue named `workqueue`
4. Run the script below to generate an access token and start the containers

```bash
export STORAGE_ACCOUNT=<storage account name>
export ACCESS_TOKEN=$(az account get-access-token --resource=https://storage.azure.com/ | jq -r .accessToken)
docker-compose -f docker-compose.yml -f docker-compose.azure.yml up
```

In a browser, navigate to [http://localhost:5000/](http://localhost:5000/).

This will start the API and MessageProcessor in "Azure" mode which will enable them to utilize Azure blob storage and storage queues. This will test the integration between the two services.

# GCP Mode

## Services
* [Google Cloud Storage](https://cloud.google.com/storage/)
* [Google PubSub](https://cloud.google.com/pubsub/)

## Steps

1. Create a new cloud storage bucket
2. Create a new PubSub topic named `workqueue`
3. Download the GCP Application Credentials json file: [instructions](https://cloud.google.com/docs/authentication/getting-started)

```bash
export GOOGLE_PROJECT_ID=<project ID>
export GOOGLE_STORAGE_BUCKET=<storage bucket name>
export GOOGLE_APPLICATION_CREDENTIALS=<path to GCP Application Credentials json file>
docker-compose -f docker-compose.yml -f docker-compose.gcp.yml up
```

In a browser, navigate to [http://localhost:5000/](http://localhost:5000/).

This will start the API and MessageProcessor in "GCP" mode which will enable them to utilize Google cloud storage and PubSub queues. This will test the integration between the two services.