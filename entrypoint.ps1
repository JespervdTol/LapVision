Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "API/API.dll"
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "CoachWeb/CoachWeb.dll"
Start-Sleep -Seconds 999999