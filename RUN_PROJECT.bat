@echo off
title Maintenance Request Manager
where dotnet >nul 2>nul
if errorlevel 1 (
  echo .NET 8 SDK was not found.
  echo Install Visual Studio 2022 with the .NET desktop development workload.
  pause
  exit /b 1
)
dotnet run --project "%~dp0MaintenanceRequestManager\MaintenanceRequestManager.csproj"
if errorlevel 1 pause
