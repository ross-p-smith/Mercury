#!/bin/bash

# PROJECT_NAME - The Google cloud project to deploy the resources into.
# BASE_RESOURCE_NAME - The base name for all resources deployed via Terraform.

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${PROJECT_NAME?}"
: "${BASE_RESOURCE_NAME?}"

# Get the directory that this script is in so the script will work regardless
# of where the user calls it from. If the scripts or its targets are moved,
# these relative paths will need to be updated.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

terraform init $DIR/../../../Terraform/src/gcp/
terraform apply -auto-approve -var project_name="$PROJECT_NAME" \
    -var base_name="$BASE_RESOURCE_NAME" \
    $DIR/../../../Terraform/src/gcp/