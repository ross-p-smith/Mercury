# Terraform
Terraform is an infrasturcture deployment tool. 
Terraform efficiently plans an infrastucture deployment process to ensure 
resources are deployed in appropriate order 
(e.g., creating a resource group before creating a storage account)

# Installing Terraform
OS X
```sh
brew update && brew install terraform
```
Windows
- [32bit](https://releases.hashicorp.com/terraform/0.12.12/terraform_0.12.12_windows_386.zip)
- [64bit](https://releases.hashicorp.com/terraform/0.12.12/terraform_0.12.12_windows_amd64.zip)


Linux
- [32bit](https://releases.hashicorp.com/terraform/0.12.12/terraform_0.12.12_linux_386.zip)
- [64bit](https://releases.hashicorp.com/terraform/0.12.12/terraform_0.12.12_linux_amd64.zip)
- [Arm](https://releases.hashicorp.com/terraform/0.12.12/terraform_0.12.12_linux_arm.zip)

# Setting a variables file

## Variables file (variables.tf)
This file creates variable prompts for the user, 
which is latered used by the `main.tf` file. 
Default values may be set here, captured via the command line, 
or set in a TF vars file.

## TF Vars file (.tfvars)
Default variables may be set in the form of:
```sh
Variable=Value
```
Terraform will automatically look for values for variables in any file with 
extension `.tfvars`. It is recommended to not check-in `.tfvars` files to your 
`git` repository, however, these files are useful for testing.

## Setting Variables from the command line
Values for variables may be set in the form of:
```sh
$ terraform apply --var "variable=value"
```

# Azure
More information on Azure resource deployment can be found 
[here](./src/azure/README.md).

In order to run 
[the Azure Terraform deploy script](../deploy/azure/scripts/terraformapply.sh), 
which will deploy the infrastructure for this solution, 
we need to first take the following steps:

# GCP
More information on GCP resource deployment can be found 
[here](./src/gcp/README.md)

# Running 
## Running with deployment scripts

### Azure
```sh
# Configure the script to be runnable
$ chmod +x ../deploy/azure/scripts/terraformapply.sh
# Run the script
$  ../deploy/azure/scripts/terraformapply.sh
```

### GCP
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
