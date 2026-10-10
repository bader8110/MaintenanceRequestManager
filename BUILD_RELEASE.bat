@echo off
title Build Maintenance Request Manager
where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET 8 SDK was not found.
  echo Install Visual Studio 2022 with the .NET desktop development workload.
  pause
  exit /b 1
)
dotnet publish "%~dp0MaintenanceRequestManager\MaintenanceRequestManager.csproj" -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
echo.
echo Build output:
echo %~dp0MaintenanceRequestManager\bin\Release\net8.0-windows\win-x64\publish
pause
