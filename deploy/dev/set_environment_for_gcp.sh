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

get_env_or_input 'Please enter a deployment location [us-central1-a]: ' DEPLOYMENT_LOCATION
get_env_or_input 'Please enter your project id: ' PROJECT_NAME
get_env_or_input 'Please enter a base resource name: ' BASE_RESOURCE_NAME

IMAGE_TAG="latest"

# Save these for later
typeset -p DEPLOYMENT_LOCATION \
           BASE_RESOURCE_NAME \
           PROJECT_NAME \
  > $DIR/gcpvariables.sh