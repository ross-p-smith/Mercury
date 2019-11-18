#!/bin/bash

deployments=()
if [[ $* == *--delete-all* ]]; then
    deployments=$(helm ls --short)
else
    deployments=$(helm ls --short --failed)
fi

# Delete all failed deployments
helm delete --purge $deployments

# Remove CustomResourceDefinitions
kubectl delete crd $(kubectl get crd -o custom-columns=:.metadata.name)

# Remove ServiceAccounts
kubectl delete sa $(kubectl get sa -o custom-columns=:.metadata.name)

# Delete Ingress
kubectl delete ing $(kubectl get ing -o custom-columns=:.metadata.name)

# Delete NGINX
kubectl delete namespaces nginx-ingress
kubectl delete clusterroles nginx-ingress-clusterrole