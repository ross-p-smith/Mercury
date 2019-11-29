#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# Get load balancer ip as endpoint for integration tests to run against 
API_BASE_ADDRESS=$($DIR/loadbalancerip.sh)

docker run -e API_BASE_ADDRESS=$API_BASE_ADDRESS ingestion-api-test:latest