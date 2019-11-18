FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY MessageProcessor/ MessageProcessor/
COPY MessageProcessor/test/UnitTest.MessageProcessor/UnitTest.MessageProcessor.csproj ./MessageProcessor/test
RUN dotnet restore "MessageProcessor/MessageProcessor.csproj"
COPY . .

# request unit tests pass as part of the build
RUN dotnet test MessageProcessor/test/UnitTest.MessageProcessor

WORKDIR "/src/MessageProcessor"
RUN dotnet build "MessageProcessor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MessageProcessor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessageProcessor.dll"]
