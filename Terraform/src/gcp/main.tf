provider "google" {
    project = var.project_name
}

data "google_storage_project_service_account" "gcs_account" {}

// Create the storage bucket
resource "google_storage_bucket" "bucket" {
    name = "${var.base_name}-${var.bucket_name}"
    bucket_policy_only = true
    force_destroy = true
}

resource "google_storage_bucket_iam_binding" "binding" {
  bucket = google_storage_bucket.bucket.name
  role   = "roles/storage.admin"
  members = ["serviceAccount:${data.google_storage_project_service_account.gcs_account.email_address}"]
}

// Create a Topic
resource "google_pubsub_topic" "topic" {
    name = "${var.base_name}-${var.queue_name}"
}

// And a subscription to the topic
resource "google_pubsub_subscription" "subscription" {
  name    = "${var.base_name}-subscription"
  topic   = google_pubsub_topic.topic.id
}

// Create a notifcation when data is uploaded to google storage
resource "google_storage_notification" "notification" {
    bucket            = google_storage_bucket.bucket.name
    payload_format    = "JSON_API_V1"
    topic             = google_pubsub_topic.topic.name
    event_types       = ["OBJECT_METADATA_UPDATE"]
    depends_on        = ["google_pubsub_topic_iam_binding.publisher"]
}

// Enable notifications by giving the correct IAM permission to the unique service account.
resource "google_pubsub_topic_iam_binding" "publisher" {
    topic       = google_pubsub_topic.topic.name
    role        = "roles/pubsub.publisher"
    members     = ["serviceAccount:${data.google_storage_project_service_account.gcs_account.email_address}"]
}

resource "google_pubsub_topic_iam_binding" "subscriber" {
    topic       = google_pubsub_topic.topic.name
    role        = "roles/pubsub.subscriber"
    members     = ["serviceAccount:${data.google_storage_project_service_account.gcs_account.email_address}"]
}

// Create kubernetes
resource "google_container_cluster" "primary" {
    project  = var.project_name
    name     = var.base_name
    location = var.cluster_location

    # We can't create a cluster with no node pool defined, but we want to only use
    # separately managed node pools. So we create the smallest possible default
    # node pool and immediately delete it.
    remove_default_node_pool = true
    initial_node_count = var.cluster_node_count
}

// Create the Node Pool in the Kubernetes Cluster
resource "google_container_node_pool" "primary_preemptible_nodes" {
    project    = google_container_cluster.primary.project
    name       = "${google_container_cluster.primary.name}-nodepool"
    location   = google_container_cluster.primary.location
    cluster    = google_container_cluster.primary.name
    node_count = google_container_cluster.primary.initial_node_count

    node_config {
        preemptible  = true
        machine_type = "n1-standard-1"

        metadata = {
            disable-legacy-endpoints = "true"
        }

        oauth_scopes = [
            "https://www.googleapis.com/auth/compute",
            "https://www.googleapis.com/auth/pubsub",
            "https://www.googleapis.com/auth/devstorage.full_control",
            "https://www.googleapis.com/auth/logging.write",
            "https://www.googleapis.com/auth/monitoring",
        ]
    }
}

// Registries are created on project creation and the terraform tools do not have the ability to create them
// This module just does some string concat to get the name
data "google_container_registry_repository" "primary" {}

// Expose GCP Container Registry to caller to support Docker push
output "gcp_container_registry" {
  value = data.google_container_registry_repository.primary.repository_url
  sensitive = true
}

output "google_project_name" {
  value = var.project_name
  sensitive = true
}

// Expose storage container name and queue
output "storage_bucket" {
  value = google_storage_bucket.bucket.name
  sensitive = true
}

output "queue_name" {
  value = google_pubsub_topic.topic.name
  sensitive = true
}

output "pubsub_subscription" {
    value = google_pubsub_subscription.subscription.name
    sensitive = true
}