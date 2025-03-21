Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "API/API.dll"

Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "App/App.dll"

Start-Sleep -Seconds 999999