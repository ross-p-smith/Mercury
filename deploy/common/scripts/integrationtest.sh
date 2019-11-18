#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

# Get load balancer ip as endpoint for integration tests to run against 
API_BASE_ADDRESS=$($DIR/loadbalancerip.sh)

# Need to delete the keda pod because it spins up before AAD pod identity can 
# be administered to keda. This will run after loadbalancerip to give aadpod
# identity pod time to create
kubectl delete pod $(kubectl get pods -o name | grep keda |  sed "s/^.\{4\}//")

docker run -e API_BASE_ADDRESS=$API_BASE_ADDRESS ingestion-api-test:latest