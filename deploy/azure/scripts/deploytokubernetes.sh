#!/bin/bash

# IMAGE_TAG - The tag of the image(s) to push.
# DISABLE_OAUTH_PERMANENTLY - Set to 'DISABLE_OAUTH_FOR_CI' to disable oauth (optional). 

# Fail when any command fails
set -e

# Ensure mandatory variables are set
: "${IMAGE_TAG?}"

# Get the directory that this script is in so the script will work regardless
# of where the user calls it from. If the scripts or its targets are moved,
# these relative paths will need to be updated.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# Create TLS certs and Keys for the correct domain $API_HOSTNAME
# Base64 encoded the contents of the cert and key files -w0 ignores the newlines in the files
API_HOSTNAME="demo.azure.com"
TLS_ORG="ingress-tls"
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -out "$TLS_ORG.crt" -keyout "$TLS_ORG.key" -subj "/CN=$API_HOSTNAME/O=$TLS_ORG"

# Mac OS doesnt have support for -w0, and instead uses -b
ENCODED_TLS_CRT=""
ENCODED_TLS_KEY=""
if [[ "$OSTYPE" == "darwin"* ]]; then
    ENCODED_TLS_CRT=$(cat $TLS_ORG.crt | base64 -b 0)
    ENCODED_TLS_KEY=$(cat $TLS_ORG.key | base64 -b 0)
else
    ENCODED_TLS_CRT=$(cat $TLS_ORG.crt | base64 -w0)
    ENCODED_TLS_KEY=$(cat $TLS_ORG.key | base64 -w0)
fi

# Initialize helm TODO: probably need to secure this
helm init --wait

helm dependency build $DIR/../../../charts/mercury

helm install --set global.cloud="azure" \
    --set global.image.tag="$IMAGE_TAG" \
    --set global.image.repository=$(terraform output acr_login_server) \
    --set aad-pod-identity.identity.id=$(terraform output aad_pod_identity_id) \
    --set aad-pod-identity.identity.clientId=$(terraform output aad_pod_identity_client_id) \
    --set ingestion-api.azure.instrumentation.key=$(terraform output instrumentation_key) \
    --set ingestion-api.azureaccount.name=$(terraform output azure_storage_account_name) \
    --set ingestion-api.storage_folder=$(terraform output storage_container) \
    --set ingestion-api.storage.queue.name=$(terraform output queue_name) \
    --set keda.aadPodIdentity="cloud-resource-access" \
    --set keda.azureaccount.name=$(terraform output azure_storage_account_name) \
    --set keda.storage_folder=$(terraform output storage_container) \
    --set keda.storage.queue.name=$(terraform output queue_name) \
    --set ingestion-api.oauth.clientId="$(terraform output oauth_client_id)" \
    --set ingestion-api.oauth.tenantId="$(terraform output oauth_tenant_id)" \
    --set ingestion-api.disable.oauth="$DISABLE_OAUTH_PERMANENTLY" \
    --set ingestion-api.hostname="$API_HOSTNAME" \
    --set ingestion-api.tls.crt="$ENCODED_TLS_CRT" \
    --set ingestion-api.tls.key="$ENCODED_TLS_KEY" \
    --wait --debug --timeout 500 \
    $DIR/../../../charts/mercury
