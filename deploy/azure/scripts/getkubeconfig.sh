#!/bin/bash

# ARM_CLIENT_ID - The client id used to authenticate to AKS.
# ARM_CLIENT_SECRET - The client secret used to authenticate to AKS.
# ARM_TENANT_ID - The tenant id of the service principal specified in ARM_CLIENT_ID.
# ARM_SUBSCRIPTION_ID - The Id of the subscription AKS lives in.
# AKS_CLUSTER_NAME - The name of the target AKS cluster.
# AKS_RESOURCE_GROUP_NAME - The name of resource group containing the target AKS cluster.
# KUBECONFIG - The location of the kubeneters config file.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${ARM_CLIENT_ID?}"
: "${ARM_CLIENT_SECRET?}"
: "${ARM_TENANT_ID?}"
: "${ARM_SUBSCRIPTION_ID?}"
: "${BASE_RESOURCE_NAME?}"
: "${KUBECONFIG?}"

# only if ARM_CLIENT_ID to facilitate development
if [ -n "$ARM_CLIENT_ID" ]
then
  az login --service-principal -u "$ARM_CLIENT_ID" -p "$ARM_CLIENT_SECRET" -t "$ARM_TENANT_ID"
  az account set -s "$ARM_SUBSCRIPTION_ID"
fi

az aks get-credentials --name $BASE_RESOURCE_NAME --resource-group $BASE_RESOURCE_NAME --overwrite-existing --file $KUBECONFIG