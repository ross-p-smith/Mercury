module "azure-provider" {
  source = "./provider"
}

locals {
  sanitized_base_name = lower(replace(var.base_name, "/[^A-Za-z0-9]/", ""))
}

/*
Uncomment and use these values when we have our own directory to grant permissions in.
data "azuread_service_principal" "aks_service_principal" {
  application_id = var.aks_service_principal_app_id
}

data "azurerm_client_config" "current" {

}*/

// Create Resource Group
resource "azurerm_resource_group" "azure_rg" {
  name     = var.base_name
  location = var.cloud_location
}

// Create Azure VNET
resource "azurerm_virtual_network" "vnet" {
  name                = var.base_name
  location            = azurerm_resource_group.azure_rg.location
  address_space       = ["10.10.0.0/24"]
  resource_group_name = azurerm_resource_group.azure_rg.name
}

// Add Subnet to Azure VNET
resource "azurerm_subnet" "subnet" {
  name                 = var.subnet_names[0]
  virtual_network_name = azurerm_virtual_network.vnet.name
  resource_group_name  = azurerm_resource_group.azure_rg.name
  address_prefix       = "10.10.0.0/24"
  service_endpoints    = ["Microsoft.Sql", "Microsoft.Storage"]

  // Work around https://github.com/Azure/AKS/issues/400 - AKS updates route table on subnet, but
  // Terraform isn't aware of this change and tries to revert it.
  lifecycle {
    ignore_changes = ["route_table_id"]
  }
}

// Create Storage Account
resource "azurerm_storage_account" "sa" {
  resource_group_name      = azurerm_resource_group.azure_rg.name
  location                 = azurerm_resource_group.azure_rg.location
  name                     = local.sanitized_base_name
  account_tier             = var.performance_tier
  account_replication_type = var.replication_type
  # optional
  account_kind              = var.storage_kind
  enable_https_traffic_only = var.https
  account_encryption_source = var.encryption_source

  # enrolls storage account into azure 'managed identities' authentication
  identity {
    type = "SystemAssigned"
  }
  network_rules {
    default_action             = "Allow"
    virtual_network_subnet_ids = [azurerm_subnet.subnet.id]
  }
}

// Create Storage Queue
resource "azurerm_storage_queue" "queue" {
  name                 = var.queue_name
  resource_group_name  = azurerm_resource_group.azure_rg.name
  storage_account_name = azurerm_storage_account.sa.name
}

// Create Storage Blob
resource "azurerm_storage_container" "blob" {
  name                  = var.blob_name
  resource_group_name   = azurerm_resource_group.azure_rg.name
  storage_account_name  = azurerm_storage_account.sa.name
  container_access_type = "private"
}

// Create Container Reg
resource "azurerm_container_registry" "container_registry" {
  name                = local.sanitized_base_name
  resource_group_name = azurerm_resource_group.azure_rg.name
  location            = azurerm_resource_group.azure_rg.location
  sku                 = "Basic"
}


// Add AKS SP to container reg
resource "azurerm_role_assignment" "cr_assign" {
  scope                = azurerm_container_registry.container_registry.id
  role_definition_name = "AcrPull"
  principal_id         = var.aks_serviceprincipal_object_id
}

// Create AKS
resource "azurerm_kubernetes_cluster" "kube" {
  name                = var.base_name
  location            = azurerm_resource_group.azure_rg.location
  resource_group_name = azurerm_resource_group.azure_rg.name
  dns_prefix          = var.base_name

  linux_profile {
    admin_username = var.linux_username
    ssh_key {
      key_data = file("${var.ssh_public_key}")
    }
  }

  agent_pool_profile {
    name            = "default"
    count           = 1
    vm_size         = "Standard_D2_v2"
    os_type         = "Linux"
    os_disk_size_gb = 30
    vnet_subnet_id  = azurerm_subnet.subnet.id
  }

  service_principal {
    client_id     = var.aks_service_principal_app_id
    client_secret = var.aks_service_principal_client_secret
  }

}

// Create a Managed Service Identity (MSI) for the pod
resource "azurerm_user_assigned_identity" "pod_identity" {
  resource_group_name = azurerm_resource_group.azure_rg.name
  location            = azurerm_resource_group.azure_rg.location

  name = var.app_identity
}

// Create a role that can manage identities
resource "azurerm_role_assignment" "aad_role" {
  scope                = azurerm_user_assigned_identity.pod_identity.id
  role_definition_name = "Managed Identity Operator"
  principal_id         = var.aks_serviceprincipal_object_id
}

// Assign MIS to Storage Account
resource "azurerm_role_assignment" "storage_identity_contributor" {
  scope                = azurerm_storage_account.sa.id
  role_definition_name = "Storage Account Contributor"
  principal_id         = azurerm_user_assigned_identity.pod_identity.principal_id
}

resource "azurerm_role_assignment" "storage_identity_blob_contributor" {
  scope                = azurerm_storage_account.sa.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.pod_identity.principal_id
}

resource "azurerm_role_assignment" "storage_identity_queue_contributor" {
  scope                = azurerm_storage_account.sa.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id         = azurerm_user_assigned_identity.pod_identity.principal_id
}

// Create Keyvault
resource "azurerm_key_vault" "keyvault" {
  name                        = local.sanitized_base_name
  location                    = azurerm_resource_group.azure_rg.location
  resource_group_name         = azurerm_resource_group.azure_rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = var.tenant_id

  sku_name = lower(var.performance_tier)

  access_policy {
    tenant_id = var.tenant_id
    object_id = azurerm_user_assigned_identity.pod_identity.principal_id
    key_permissions = [
      "list",
      "get",
    ]
    secret_permissions = [
      "list",
      "get",
    ]
  }

  tags = {
    environment = "Production"
  }
}

resource "azurerm_application_insights" "api_insights" {
  name                = var.base_name
  location            = azurerm_resource_group.azure_rg.location
  resource_group_name = azurerm_resource_group.azure_rg.name
  application_type    = "web"
}

// Expose ACR Login Server to caller to support Docker push
output "acr_login_server" {
  value = azurerm_container_registry.container_registry.login_server
  sensitive = true
}

// Expose Service Identity IDs for pod AAD
output "aad_pod_identity_id" {
  value = azurerm_user_assigned_identity.pod_identity.id
  sensitive = true
}
output "aad_pod_identity_client_id" {
  value = azurerm_user_assigned_identity.pod_identity.client_id
  sensitive = true
}

// Expose Application Insights Instrumentation key
output "instrumentation_key" {
  value = azurerm_application_insights.api_insights.instrumentation_key
  sensitive = true
}

// Expose storage account details
output "azure_storage_account_name" {
  value = azurerm_storage_account.sa.name
  sensitive = true
}

// Expose storage container name and queue
output "storage_container" {
  value = var.blob_name
  sensitive = true
}

output "queue_name" {
  value = var.queue_name
  sensitive = true
}

// Expose the oAuth client and tenant id
output "oauth_client_id" {
  value = var.oauth_client_id
  sensitive = true
}

output "oauth_tenant_id" {
  value = var.oauth_tenant_id
  sensitive = true
}