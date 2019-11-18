# Azure Resource creator

`main.tf` contains methods to create the following resources
- Resource Group
- VNet
- AKS
- Storage Queue
- Blob Storage Account
- Container Registry


# Requirements:

## Install Terraform
See [here](../../README.md#installing-terraform)


## Install Azure CLI on Unix based System (OS X, Deb Linux, WSL)

OS X
```sh
brew update && brew install azure-cli
```

Ubuntu (WSL)
```sh
$ curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

## Authenticate with Azure CLI

```sh
$ az login
```
This will redirect you to the browser

or

```sh
$ az login -u <username> -p <password>
```

# Running

In order to run 
[the Azure Terraform deploy script](../../../deploy/azure/scripts/terraformapply.sh), 
that will deploy the infrastructure for this solution,
 we need to first take the following steps:

1. Create an Azure service principal for AKS to authenticate to ACR.
    - To create a service principal see 
    [here](../../../docs/managed-identities.md#azure-managed-service-identities)
    .
2. Create an SSH keypair as the administrative credentials in the AKS cluster.

## SSH key:
```sh
$ ssh-keygen -f <filename>
```

This will give 2 files, `filename` and `filename.pub`
`filename.pub` is your public key

Place the path to `filename.pub` for the Terraform variable `ssh_public_key`.

## Running with deployment script

```sh
# Configure the script to be runnable
$ chmod +x ../../../deploy/azure/scripts/terraformapply.sh
# Run the script
$  ../../../deploy/azure/scripts/terraformapply.sh
```

## Running without the deployment script
```sh
$ terraform init
$ terraform apply
```
