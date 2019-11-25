#!/bin/bash

# ARM_CLIENT_ID - The client id used to push docker images to ACR.
# ARM_CLIENT_SECRET - The client secret used to push docker images to ACR.
# ARM_SUBSCRIPTION_ID - The Id of the subscription ACR lives in.
# ARM_TENANT_ID - The tenant id of the service principal specified in ARM_CLIENT_ID.
# IMAGE_TAG - The tag of the image(s) to push.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${ARM_CLIENT_ID?}"
: "${ARM_CLIENT_SECRET?}"
: "${ARM_SUBSCRIPTION_ID?}"
: "${ARM_TENANT_ID?}"
: "${IMAGE_TAG?}"

# Get acr_login_server output variable from terraform state.
# This was set by the terraform apply command.
export ACR_REGISTRY_URI=$(terraform output acr_login_server)

# ACR login only works with the registry name, not the registry URI.
# Parse the registry name out of the URI.
registry_name=$(echo $ACR_REGISTRY_URI | awk -F "." '{print $1}')

# Log into the ACR instance via az-cli
# only if ARM_CLIENT_ID to facilitate development
if [ -n "$ARM_CLIENT_ID" ]
then
  az login --service-principal -u "$ARM_CLIENT_ID" -p "$ARM_CLIENT_SECRET" -t "$ARM_TENANT_ID"
  az account set -s "$ARM_SUBSCRIPTION_ID"
fi

az acr login --name "$registry_name"

# Tag the built image specifically for Azure ACR before pushing
docker tag ingestion-api $ACR_REGISTRY_URI/ingestion-api:$IMAGE_TAG
docker tag messageprocessor $ACR_REGISTRY_URI/messageprocessor:$IMAGE_TAG
#docker tag kedacore/keda:1.0.0 $ACR_REGISTRY_URI/keda:$IMAGE_TAG

docker push $ACR_REGISTRY_URI/ingestion-api:$IMAGE_TAG
docker push $ACR_REGISTRY_URI/messageprocessor:$IMAGE_TAG
#docker push $ACR_REGISTRY_URI/keda:$IMAGE_TAG
