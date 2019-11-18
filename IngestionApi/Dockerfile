FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . ./
COPY ./CloudProviders ./CloudProviders
COPY IngestionApi/IngestionApi.sln ./IngestionApi/IngestionApi.sln
COPY IngestionApi/IngestionApi/ ./IngestionApi/IngestionApi/
COPY IngestionApi/test/UnitTest.IngestionApi/UnitTest.IngestionApi.csproj ./IngestionApi/test/UnitTest.IngestionApi/UnitTest.IngestionApi.csproj
COPY IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj ./IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj
RUN dotnet restore IngestionApi/IngestionApi.sln

RUN dotnet publish IngestionApi/IngestionApi/IngestionApi.csproj -c Release -o out

# request unit tests pass as part of the build
RUN dotnet test IngestionApi/test/UnitTest.IngestionApi

#run integration test
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS IntegrationTest
WORKDIR /app
COPY --from=build-env app .
COPY IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj .IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj
RUN dotnet restore ./IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj
RUN dotnet publish IngestionApi/test/IntegrationTest.IngestionApi/IntegrationTest.IngestionApi.csproj -c Release -o out
ENTRYPOINT ["dotnet", "test", "./IngestionApi/test/IntegrationTest.IngestionApi"]

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build-env app .
WORKDIR /app/IngestionApi/IngestionApi/out
ENTRYPOINT ["dotnet", "IngestionApi.dll"]
