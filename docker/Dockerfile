# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["LedgerLink.csproj", "./"]
RUN dotnet restore "LedgerLink.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "LedgerLink.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LedgerLink.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LedgerLink.dll"]
