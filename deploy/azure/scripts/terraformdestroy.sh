#!/bin/bash

# ARM_CLIENT_ID - The client id used to run Terraform.
# ARM_CLIENT_SECRET - The client secret used to run Terraform.
# ARM_SUBSCRIPTION_ID - The Id of the subscription Terraform will deploy into.
# ARM_TENANT_ID - The tenant id of the service principal specified in ARM_CLIENT_ID.

# BASE_RESOURCE_NAME - The base name for all resources deployed via Terraform.
# DEPLOYMENT_LOCATION - The Azure location to deploy the resources to.

# AKS_SP_APP_ID - The app id of the Service Principal used by AKS.
# AKS_SP_OBJECT_ID - The object id of the Service Principal used by AKS.
# AKS_SP_CLIENT_SECRET - The client secret of the Service Principal used by AKS.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${ARM_CLIENT_ID?}"
: "${ARM_CLIENT_SECRET?}"
: "${ARM_TENANT_ID?}"
: "${ARM_SUBSCRIPTION_ID?}"
: "${BASE_RESOURCE_NAME?}"
: "${DEPLOYMENT_LOCATION?}"
: "${AKS_SP_APP_ID?}"
: "${AKS_SP_OBJECT_ID?}"
: "${AKS_SP_CLIENT_SECRET?}"

# Get the directory that this script is in so the script will work regardless
# of where the user calls it from. If the scripts or its targets are moved,
# these relative paths will need to be updated.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

export ARM_CLIENT_ID="$ARM_CLIENT_ID"
export ARM_CLIENT_SECRET="$ARM_CLIENT_SECRET"
export ARM_SUBSCRIPTION_ID="$ARM_SUBSCRIPTION_ID"
export ARM_TENANT_ID="$ARM_TENANT_ID"

terraform init $DIR/../../../Terraform/src/azure/
terraform destroy -auto-approve -var cloud_location="$DEPLOYMENT_LOCATION" \
    -var base_name="$BASE_RESOURCE_NAME" \
    -var ssh_public_key="$DIR/pubkey.pub" \
    -var tenant_id="$ARM_TENANT_ID" \
    -var aks_service_principal_app_id="$AKS_SP_APP_ID" \
    -var aks_serviceprincipal_object_id="$AKS_SP_OBJECT_ID" \
    -var aks_service_principal_client_secret="$AKS_SP_CLIENT_SECRET" \
    $DIR/../../../Terraform/src/azure/