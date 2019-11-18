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

API_HOSTNAME="demo.gcp.com"
TLS_ORG="ingress-tls"
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -out "$TLS_ORG.crt" -keyout "$TLS_ORG.key" -subj "/CN=$API_HOSTNAME/O=$TLS_ORG"
ENCODED_TLS_CRT=$(cat $TLS_ORG.crt | base64 -w0)
ENCODED_TLS_KEY=$(cat $TLS_ORG.key | base64 -w0)

ENCODED_GCP_CLIENT_SECRET=$(echo "" | base64)

kubectl --namespace kube-system create sa tiller

kubectl create clusterrolebinding tiller \
  --clusterrole cluster-admin \
  --serviceaccount=kube-system:tiller

kubectl create secret generic pubsub-secret \
  --from-file=GOOGLE_APPLICATION_CREDENTIALS_JSON=$GOOGLE_CLOUD_KEYFILE_JSON \
  --from-literal=PROJECT_ID=$PROJECT_NAME
  
# Initialize helm TODO: probably need to secure this
helm init --service-account=tiller --wait

helm dependency build $DIR/../../../charts/mercury

helm install --set global.cloud="gcp" \
    --set global.image.tag="$IMAGE_TAG" \
    --set global.image.repository=$(terraform output gcp_container_registry) \
    --set aad-pod-identity.enabled="false" \
    --set ingestion-api.hostname="$API_HOSTNAME" \
    --set ingestion-api.tls.crt="$ENCODED_TLS_CRT" \
    --set ingestion-api.tls.key="$ENCODED_TLS_KEY" \
    --set ingestion-api.gcp.project_id=$(terraform output google_project_name) \
    --set ingestion-api.storage_folder=$(terraform output storage_bucket) \
    --set ingestion-api.storage.queue.name=$(terraform output queue_name) \
    --set ingestion-api.disable.oauth="$DISABLE_OAUTH_PERMANENTLY" \
    --set ingestion-api.oauth.clientId="" \
    --set ingestion-api.oauth.clientSecret="$ENCODED_GCP_CLIENT_SECRET" \
    --set ingestion-api.gcp.pubsub.subscriptionName=$(terraform output pubsub_subscription) \
    --set keda.gcp.project_id=$(terraform output google_project_name) \
    --set keda.gcp.pubsub.subscriptionName=$(terraform output pubsub_subscription) \
    --set keda.storage_folder=$(terraform output storage_bucket) \
    --set keda.storage.queue.name=$(terraform output queue_name) \
    --wait \
    $DIR/../../../charts/mercury