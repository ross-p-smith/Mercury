variable "aks_service_principal_app_id" {
  description = "App ID of AKS service principal"
  type        = string
}

variable "aks_service_principal_client_secret" {
  description = "Secret of AKS service principal"
  type        = string
}

variable "ssh_public_key" {
  description = "Path to ssh public key file"
  type        = string
}

# NEEDED UNTIL WE HAVE OUR OWN AD DIRECTORY
variable "tenant_id" {
  description = "The ID of the tenant"
  type        = string
}

variable "aks_serviceprincipal_object_id" {
  description = "The name of the Azure virtual network"
  type        = string
}

variable "base_name" {
  description = "This name will be used for all resource deployments and the resource group"
  type        = string
}

### The following variables have defaults

variable "oauth_client_id" {
  description = "The client id for the oAuth web registration"
  type        = string
  default     = "5de50128-84b4-48b3-a466-e27d2c975f6f"
}

variable "oauth_tenant_id" {
  description = "The tenant id for the oAuth web registration"
  type        = string
  default     = "72f988bf-86f1-41af-91ab-2d7cd011db47"
}

variable "cloud_location" {
  description = "Which location to spin up resources into"
  type        = string
  default     = "centralus"
}

variable "subnet_names" {
  description = "List of the subnet names for the Azure virtual network"
  type        = list(string)
  default     = ["subnet0"]
}

variable "storage_kind" {
  description = "Storage account types that determine available features and pricing of Azure Storage. Use StorageV2 when possible."
  type        = string
  default     = "StorageV2"
}

variable "blob_name" {
  description = "The name of the blob used for file upload"
  type        = string
  default     = "workfiles"
}

variable "queue_name" {
  description = "The name of the queue used for storing jobs"
  type        = string
  default     = "workqueue"
}

variable "linux_username" {
  description = "Username of aks linux user"
  type        = string
  default     = "azureuser"
}

variable "performance_tier" {
  description = "Determines the level of performance required."
  type        = string
  default     = "Standard"
}

variable "replication_type" {
  description = "Defines the type of replication to use for this storage account. Valid options are LRS*, GRS, RAGRS and ZRS."
  type        = string
  default     = "LRS"
}

variable "encryption_source" {
  description = "Determines the source that will manage encryption for the storage account. Valid options are Microsoft.Storage and Microsoft.Keyvault."
  type        = string
  default     = "Microsoft.Storage"
}

variable "https" {
  description = "Boolean flag which forces HTTPS in order to ensure secure connections."
  type        = bool
  default     = true
}

variable "app_identity" {
  description = "Name of the identity used to obtain keyvault secrets"
  type        = string
  default     = "ingestion-app"
}