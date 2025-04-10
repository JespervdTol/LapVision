﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app
EXPOSE 5082
EXPOSE 7234

FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /LapVision

COPY . .

ENV DOTNET_ROOT="C:\\Program Files\\dotnet"
ENV PATH="C:\\Windows\\System32;C:\\Program Files\\dotnet;"

RUN cmd /C """"C:\Program Files\dotnet\dotnet.exe""" --info || echo dotnet not found, continuing..."

RUN cmd /C """"C:\Program Files\dotnet\dotnet.exe""" workload restore || echo Workload restore failed, continuing..."
RUN cmd /C """"C:\Program Files\dotnet\dotnet.exe""" restore LapVision.sln || echo Restore failed, continuing..."

RUN cmd /C """"C:\Program Files\dotnet\dotnet.exe""" build LapVision.sln -c %BUILD_CONFIGURATION% -o /app/build || echo Build failed, continuing..."

RUN cmd /C """"C:\Program Files\dotnet\dotnet.exe""" publish API/API.csproj -c %BUILD_CONFIGURATION% -o /app/publish/API /p:UseAppHost=false || echo Publish failed, continuing..."

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish/API ./API
COPY API/appsettings.json ./API/appsettings.json
COPY publish ./App
COPY entrypoint.ps1 .
COPY API/cert.pfx ./cert.pfx

SHELL ["powershell", "-Command"]
ENTRYPOINT ["powershell", "-File", "entrypoint.ps1"]