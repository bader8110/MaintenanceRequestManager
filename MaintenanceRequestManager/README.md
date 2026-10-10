# Maintenance Request Manager

A Windows Forms application for submitting, tracking, assigning, and closing maintenance requests.

## Requirements

- Windows 10 or Windows 11
- Visual Studio 2022 with **.NET desktop development** workload
- .NET 8 SDK

## Run with Visual Studio

1. Open `MaintenanceRequestManager.sln`.
2. Wait for Visual Studio to load the project.
3. Press `F5` or click **Start**.

You can also double-click `RUN_PROJECT.bat` from the project package.

## Run from Terminal

```powershell
dotnet restore
dotnet build
dotnet run --project .\MaintenanceRequestManager\MaintenanceRequestManager.csproj
```

To make a Windows release build, double-click `BUILD_RELEASE.bat`.

## Demo accounts

- Administrator: `admin` / `admin123`
- Regular user: `user` / `user123`

## Data file

The application creates its data file automatically at:

`%LOCALAPPDATA%\MaintenanceRequestManager\data.json`

Delete that file only when you want to reset the sample data.

## Main features

- Role-based login for administrator and regular user
- Logout button for switching between user and administrator accounts
- Complete English and Arabic interface
- Light and dark appearance modes
- Saved language, theme, and logout-confirmation preferences
- Submit a maintenance request
- Edit requests (administrators can edit any request; users can edit their own new requests)
- Delete requests with a confirmation prompt (same role rules as editing)
- Search by ID, title, or location; filter by status, priority, and category
- Refresh and clear-filter actions
- Color-coded status and priority values
- Update status, priority, technician, and admin notes
- Dashboard statistics
- Export the visible request list to CSV
- Local JSON data persistence without external NuGet packages

## Settings file

The application saves language and appearance preferences at:

`%LOCALAPPDATA%\MaintenanceRequestManager\settings.json`
