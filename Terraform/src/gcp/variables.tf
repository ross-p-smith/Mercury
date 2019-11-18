variable "project_name" {
    description = "Google Cloud project name"
    type        = string
}

variable "base_name" {
  description = "This name will be used for most resource deployments"
  type        = string
}

### The following variables have defaults

variable "cluster_location" {
    description = "Location of the GCP Container Cluster"
    type = string
    default = "us-central1-a"
}

variable "cluster_node_count" {
    description = "Number of Nodes to be created"
    type = number
    default = 1
}

variable "bucket_name" {
    description = "name of bucket"
    type        = string
    default     = "workfiles"
}

variable "queue_name" {
    description = "name of queue"
    type        = string
    default     = "workqueue"
}