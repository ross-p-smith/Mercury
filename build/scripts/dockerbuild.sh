#!/bin/bash

# Fail when any command fails
set -e

# Get the directory that this script is in so the script will work regardless
# of where the user calls it from. If the scripts or its targets are moved,
# these relative paths will need to be updated.
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# Build without a container registry-specific tag so the build can be used
# with multiple clouds.
docker build -t ingestion-api $DIR/../.. -f $DIR/../../IngestionApi/Dockerfile
docker build -t messageprocessor $DIR/../../ -f $DIR/../../MessageProcessor/Dockerfile
docker build -t ingestion-api-test $DIR/../.. -f $DIR/../../IngestionApi/Dockerfile --target=IntegrationTest
