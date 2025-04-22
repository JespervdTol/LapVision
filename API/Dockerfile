# -------- Base Image --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app
EXPOSE 5082
EXPOSE 7234

# -------- Build Image --------
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build

# Accept build configuration via ARG (default to Release)
ARG CONFIGURATION=Release

WORKDIR /src

# Copy solution and project files separately for Docker layer caching
COPY LapVision.Docker.slnf ./LapVision.Docker.slnf
COPY LapVision.sln ./LapVision.sln

COPY API/API.csproj API/
COPY Contracts/Contracts.csproj Contracts/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Model/Model.csproj Model/

# Restore only the filtered projects listed in the .slnf
RUN dotnet restore LapVision.Docker.slnf

# Copy everything else
COPY . ./

# Build and publish the API project
WORKDIR /src/API
RUN dotnet publish API.csproj -c %CONFIGURATION% -o /app/publish/API /p:UseAppHost=false

# -------- Runtime Image --------
FROM base AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish/API ./API

# Copy configuration and certificate
COPY API/appsettings.json ./API/appsettings.json
COPY API/cert.pfx ./cert.pfx

# Optional: Copy published frontend output (if needed)
COPY publish ./App

# Entrypoint script
COPY entrypoint.ps1 .

# Use PowerShell and run the script
SHELL ["powershell", "-Command"]
ENTRYPOINT ["powershell", "-File", "entrypoint.ps1"]