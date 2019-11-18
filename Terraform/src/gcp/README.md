# GCP VPC creation
`main.tf` contains methods to create the following resources
- Storage Bucket
- Pub/Sub Topic
- Pub/Sub Subscription
- GKE container


# Requirements:

## Install Terraform
See [here](../../README.md#installing-terraform)

In order to run 
[the GCP Terraform deploy script](../deploy/gcp/scripts/terraformapply.sh),
 that will deploy the infrastucture for this solution, 
 we need to first take the following steps:
1. Install the [GCloud SDK](https://cloud.google.com/sdk/)
2. Authenticate with the `gcloud cli`
    ```sh
    $ gcloud auth login
    ```
3. Set the G-Cloud Project
    ```sh
    $ gcloud config set project PROJECT_ID
    ```
4. Create a GCP Service Account:
    - To create a Service account see 
    [here](../docs/managed-identities.md#google-service-account)
5. Set the enivronement variable `$GOOGLE_APPLICATION_CREDENTIALS`
    - This is achieved 
    [here](../docs/managed-identities.md#google-application-credentials)

## Running with deployment script

```sh
# Configure the script to be runnable
$ chmod +x ../../../deploy/gcp/scripts/terraformapply.sh
# Run the script
$  ../../../deploy/gcp/scripts/terraformapply.sh
```

## Running without the deployment script
```sh
$ terraform init
$ terraform apply
```