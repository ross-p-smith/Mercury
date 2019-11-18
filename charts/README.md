# Helm 
Helm is a templating tool for Kubernetes resource definitions. This allows for 
values to be placed into Kubernetes `yaml` files at deployment time.

## Deploying
- [Azure](../deploy/azure/scripts/deploytokubernetes.sh)
- [GCP](../deploy/gcp/scripts/deploytokubernetes.sh)

## Helm Notation

### Variables

Variables are defined as:

```json
{{ .Values.global.cloud }}
```

Variable values can be set in `values.yaml` files or in `helm cli`


**Global Variables**
```sh
$ helm template --set global.cloud="azure" .
```

**Chart Scoped Variables**
```sh
$ helm template --set <chart-name>.variable="" .
```

### If statements

Conditional templating can be defined as an `if` statement shown below:

```json
{{ if eq .Values.global.cloud "azure" }}
```

## Mercury
The Mecury helm chart deploys all of the resources necessary for running 
Mercury in Kubernetes. The following resources are deployed:
- Ingestion API as a deployment
- NGINX Service
- Ingress Route
- Managed Identities

*Globally Required Helm Variables:*
- `global.cloud`
    - Cloud that you are currently on `GCP` or `Azure`
- `global.image.tag`
    - The Image tag to be used for the image, typically `latest`
- `global.image.repository`
    - The Image repository for the Ingestion API and KEDA to pull from

## AAD Pod Identities
This creates Azure Active Directory Pod Identity bindings. AAD Pod Identities 
allow for on-behalf-of access to PaaS resources. More information on Managed 
Service Identities can be found 
[here](../docs/managed-identities.md#configuring-managed-identities).


*Required Helm Variables:*
- `aad-pod-identity.identity.id`
    - Service Principal Object ID to be used for AAD Pod Identities
- `aad-pod-identity.identity.clientId`
    - Service Principal Client ID to be used for AAD Pod Identities


## Ingestion API
The ingestion API directory found [here](./ingestion-api). 

*Required Helm Variables:*
- `ingestion-api.storage_folder`
    - Azure Storage Blob or GCP Bucket name
- `ingestion-api.storage.queue.name`
    - Azure Queue or GCP Pub/Sub name
- `ingestion-api.oauth.clientId`
    - Azure or GCP Oauth client ID
- `ingestion-api.disable.oauth`
    - Flag to disable to Oauth for tests
- `ingestion-api.hostname`
    - Hostname of the API
- `ingestion-api.tls.crt`
    - Base64 encoded TLS certificate
- `ingestion-api.tls.key`
    - Base64 encoded TLS key

- Azure
    - `ingestion-api.azure.instrumentation.key`
        - Azure App Insights API key
    - `ingestion-api.azureaccount.name`
        - Azure Storage Account Name
    - `ingestion-api.oauth.tenantId`
        - Azure App tenant ID
- GCP
    - `ingestion-api.gcp.project_id`
        - GCP project ID
    - `ingestion-api.oauth.clientSecret`
        - GCP client secret, which has been Base64 encoded
    - `ingestion-api.gcp.pubsub.subscriptionName`
        - GCP Pub/Sub Subscription name


The core components 
deployed are:

### Deployment
This deploys the API, with the container image, and exposed ports.

### GCP Secret
This is the GCP oAuth client secret. More information on this can be found in 
the Ingestion API readme [here](../IngestionApi/README.md#configuring-oauth). 
This value is base64 encoded before being added.

### Ingress Route
This connects the NGNIX load balancer to the Ingestion API service container.

### NGINX and NGINX Service
The NGINX service exists to allow for TLS termination. This service runs the 
NGINX Gateway. 

### Service
This service exists to run the Ingestion API deployment. The Ingress Route 
connects the NGNIX ingress to this.

### TLS
This is deploys a TLS certifate as a Kuberenets Secret. As all Kubernetes 
secrets, this is base64 encoded.

## KEDA
This deploys the resources necessary for KEDA to run in kubernetes. KEDA is an 
event driven auto-scaler for kubernetes. More information about KEDA can be 
found [here](../KEDA/README.md).

*Required Helm Variables:*
- `keda.storage_folder`
    - Azure Storage Blob or GCP Bucket Name
- `keda.storage.queue.name`
    - Azure Storage Queue or GCP Pub/Sub
- Azure
    - `keda.azureaccount.name`
        - Azure Storage Account Name
- GCP
    - `keda.gcp.project_id`
        - GCP Project ID
    - `keda.gcp.pubsub.subscriptionName`
        - GCP Pub/Sub Subscription Name 

## Chart
This metadata file is used to generate the built helm dependencies.

## Values
Default helm variable values may be defined here.

## _helpers.tpl file
These files are used for defining resource prefixes. This will deploy all 
kubernetes resources with the same prefix followed by the `-<resourcename>`
