#!/bin/bash

# BASE_RESOURCE_NAME - The name used for most resources
# DEPLOYMENT_LOCATION - The location to deploy the resources into
# PROJECT_NAME - The Google cloud project to deploy the resources into
# GOOGLE_CLOUD_KEYFILE_JSON - The path to the GCP credentials JSON file.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${BASE_RESOURCE_NAME?}"
: "${DEPLOYMENT_LOCATION?}"
: "${PROJECT_NAME?}"
: "${GOOGLE_CLOUD_KEYFILE_JSON?}"

PATH=$PATH:/opt/google-cloud-sdk/bin;

gcloud container clusters get-credentials $BASE_RESOURCE_NAME --zone $DEPLOYMENT_LOCATION

