# Message Processor

The Message Processor is a lightweight cloud agnostic service written in .NET
Core. The Message Processor watches a queue (e.g., Azure Storage Queue, GCP 
Pub/Sub subscription), when a new item arrives, the Message Processor, reads
the message contents, downloads the correlated file, and performs a pre-defined,
task on the file. Currently, when a file is downloaded, the Message Processor
only uploads an acknowledgment file (`NotifyReceived.txt`) to the cloud 
agnostic storage, however, this application is designed to be higly 
customizable.

The Message Processor is currently used in coordination with 
[KEDA](../KEDA/README.md). KEDA is a Kubernetes Event Driven Autoscaler, which
is used to automatically auto-scale when an event happens. In this scenario,
when a new item is added to the queue.

Current cloud support:

- [x] Azure
- [x] Google Cloud


## Development Environment Setup

- Install Visual Studio or Visual Studio Code
- Install Docker 
    - [Direct download for mac](https://download.docker.com/mac/stable/Docker.dmg)
    - [Direct download for windows](https://download.docker.com/win/stable/Docker%20Desktop%20Installer.exe)
    - [Instructions for debian/ WSL](https://www.digitalocean.com/community/tutorials/how-to-install-and-use-docker-on-ubuntu-16-04)
- Install `kubectl` (Use `$ az aks install-cli`)

You will also need the appropriate sdk for your target cloud:

- Install `azure-cli` 
    - [Instructions for mac](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos?view=azure-cli-latest)
    - [Instructions for windows](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest)
    - [Instructions for linux/WSL](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-apt?view=azure-cli-latest)

- Install `gcloud sdk` 
    - [Instructions for mac](https://cloud.google.com/sdk/docs/quickstart-macos)
    - [Instructions for windows](https://cloud.google.com/sdk/docs/quickstart-windows)
    - [Instructions for linux/WSL](https://cloud.google.com/sdk/docs/quickstart-linux)
        - Do not install with a package manager, download the binary directly

# Application Configuration
**IMPORTANT** 

For the Ingestion API to work properly, it must be configured with values for 
resources (e.g., Azure Queue). To configure these, follow the documentation 
[here](../docs/application-configuration.md). 

# Running Locally
## Non-Docker
In this directory run:

```sh
$ dotnet build
$ dotnet run
```

## Docker

In the parent directory: 
### Build the image
```sh
$ docker build -t messageprocessor . -f MessageProcessor/Dockerfile --target=runtime
```

### Run the image
```sh
$ docker run messageprocessor
```

More information on running locally can be found [here](../docs/running-locally.md).

# Debugging

## .NET (non docker)
In VS Code:

1. Open this directory
2. Select *Debug->Start Debugging*
    - *(if not already configured)*
    - Select *.NET core*
    - Select the *MessageProcessor.csproj*
5. A browser will launch, any breakpoints will now be triggered

## Docker

1. Install the docker extension for VS Code
2. Build and run the *messageprocessor* as detailed above 
3. In the VS Code Docker extension
    - Select the running container
    - Right-click and attach VS Code

# Testing

# Unit Tests

### Message Processor Unit Test
1. Attempts to download a file, failing since it doesnt exist
2. The file is uploaded and the metadata is enqueued
3. The file is downloaded and ensures that file url matches the enqueued
metadata's URL

*Running*
```sh
$ dotnet test MessageProcessor/test/UnitTest.MessageProcessor
```
## Integration Tests
The Message Processor must be running first for Integration Testing. To run 
the Message Processor see [here](#Running-Locally).

For more information on running Integration Tests with the Ingestion API. See
[here](../IngestionApi/README.md#Integration-Tests).

## Fixing Failed Tests
When a test with multiple tests fail, the failed tests will show up in red.
Tests fails on `assert(...)` statements. To fix the error, tests can be 
debugged individually and the failed assertion should be logged with the exact 
line number. A good starting point is to traceback the failed assertion value.



