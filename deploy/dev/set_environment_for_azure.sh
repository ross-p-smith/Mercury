#!/bin/bash

# Get the directory that this script is in so the script will work regardless
# of where the user calls it from. If the scripts or its targets are moved,
# these relative paths will need to be updated.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# get_env_or_input {prompt} {variable}
function get_env_or_input() {
  if [ -z "${!2}" ]
  then
    read -p "$1" $2
  else
    printf "${1}${!2}\n"
  fi
}

get_env_or_input 'Please enter a deployment location: ' DEPLOYMENT_LOCATION
get_env_or_input 'Please enter a base resource name: ' BASE_RESOURCE_NAME
get_env_or_input 'Please enter the AKS Service Principal Tenant ID: ' ARM_TENANT_ID
get_env_or_input 'Please enter the AKS Service Principal Application ID: ' AKS_SP_APP_ID
get_env_or_input 'Please enter the AKS Service Principal Client Secret: ' AKS_SP_CLIENT_SECRET
get_env_or_input 'Please enter the AKS Service Principal Object ID: ' AKS_SP_OBJECT_ID
get_env_or_input 'Please enter the Application oAuth Client ID: ' AKS_OAUTH_CLIENT_ID
get_env_or_input 'Please enter the Application oAuth Tenant ID: ' AKS_OAUTH_TENANT_ID

AKS_CLUSTER_NAME=$BASE_RESOURCE_NAME
AKS_RESOURCE_GROUP_NAME=$BASE_RESOURCE_NAME

IMAGE_TAG="latest"

# Save these for later
typeset -p DEPLOYMENT_LOCATION \
           BASE_RESOURCE_NAME \
           ARM_TENANT_ID \
           AKS_SP_APP_ID \
           AKS_SP_CLIENT_SECRET \
           AKS_SP_OBJECT_ID \
           AKS_OAUTH_CLIENT_ID \
           AKS_OAUTH_TENANT_ID \
  > $DIR/azurevariables.sh