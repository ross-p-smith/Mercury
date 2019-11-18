#!/bin/bash

# Fail when any command fails
set -e

# Used for color output
CYAN='\033[0;36m'
NC='\033[0m'

function set_dir() {
  # Get the directory that this script is in so the script will work regardless
  # of where the user calls it from. If the scripts or its targets are moved,
  # these relative paths will need to be updated.
  DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
}

function run_script() {
  set_dir
  printf "${CYAN}${1}${NC}\n"
  source $DIR/../../$1
}

# Reload variables from the last run if they exist
set_dir
if [ -f "$DIR/azurevariables.sh" ]; then
  source $DIR/azurevariables.sh
fi

run_script /deploy/dev/set_environment_for_azure.sh

[[ $* != *--skip-build* ]] && run_script build/scripts/dockerbuild.sh

[[ $* != *--skip-terraform* ]] && run_script deploy/azure/scripts/terraformapply.sh

run_script deploy/azure/scripts/dockerpush.sh
run_script deploy/azure/scripts/getkubeconfig.sh
run_script deploy/azure/scripts/deploytokubernetes.sh

[[ $* == *--run-integration* ]] && run_script deploy/common/scripts/integrationtest.sh
