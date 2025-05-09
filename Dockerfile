# -------- Base Image -------- 
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app
EXPOSE 5082
EXPOSE 7234
EXPOSE 7222
EXPOSE 5223 

# -------- Build Image --------
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build

ARG CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY LapVision.sln ./LapVision.sln
COPY LapVision.Docker.slnf ./LapVision.Docker.slnf
COPY API/API.csproj API/
COPY CoachWeb/CoachWeb.csproj CoachWeb/
COPY Contracts/Contracts.csproj Contracts/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Model/Model.csproj Model/

# Restore filtered solution
RUN dotnet restore LapVision.Docker.slnf

# Copy everything
COPY . .

# --- Build API ---
WORKDIR /src/API
RUN dotnet publish -c %CONFIGURATION% -o /app/publish/API /p:UseAppHost=false

# --- Build CoachWeb ---
WORKDIR /src/CoachWeb
RUN dotnet publish -c %CONFIGURATION% -o /app/publish/CoachWeb /p:UseAppHost=false

# -------- Runtime Image --------
FROM base AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish/API ./API
COPY --from=build /app/publish/CoachWeb ./CoachWeb

# Copy any required cert or shared resources
COPY API/cert.pfx ./cert.pfx

# Entrypoint script (optional)
COPY entrypoint.ps1 .

# Use PowerShell to launch both apps
SHELL ["powershell", "-Command"]
ENTRYPOINT ["powershell", "-File", "entrypoint.ps1"]