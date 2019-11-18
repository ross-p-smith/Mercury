#!/bin/bash

# IMAGE_TAG - The tag of the image(s) to push.
# PROJECT_NAME - The Google cloud project to deploy the resources into.
# GOOGLE_CLOUD_KEYFILE_JSON - The path to the GCP credentials JSON file.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${IMAGE_TAG?}"
: "${PROJECT_NAME?}"
: "${ARM_SUBSCRIPTION_ID?}"
: "${GOOGLE_CLOUD_KEYFILE_JSON?}"

PATH=$PATH:/opt/google-cloud-sdk/bin;

# Get acr_login_server output variable from terraform state.
# This was set by the terraform apply command.
export GCP_REGISTRY_URI=$(terraform output gcp_container_registry)

gcloud auth activate-service-account --key-file $GOOGLE_CLOUD_KEYFILE_JSON --project $PROJECT_NAME
docker login -u _json_key --password-stdin $GCP_REGISTRY_URI < $GOOGLE_CLOUD_KEYFILE_JSON

# Tag the built image specifically for Azure ACR before pushing
docker tag ingestion-api $GCP_REGISTRY_URI/ingestion-api:$IMAGE_TAG
docker tag messageprocessor $GCP_REGISTRY_URI/messageprocessor:$IMAGE_TAG
docker tag keda $GCP_REGISTRY_URI/keda:$IMAGE_TAG

docker push $GCP_REGISTRY_URI/ingestion-api:$IMAGE_TAG
docker push $GCP_REGISTRY_URI/messageprocessor:$IMAGE_TAG
docker push $GCP_REGISTRY_URI/keda:$IMAGE_TAG