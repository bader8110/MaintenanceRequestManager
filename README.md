# Maintenance Request Manager

A desktop application for recording and managing maintenance requests. Residents can submit and follow their requests, while administrators can review, update, assign, and manage the full request list.

## Project overview

The application was developed as a Software Engineering course project using C# and Windows Forms. Its goal is to organize maintenance reports in one simple desktop system instead of relying on informal messages or paper records.

### Main capabilities

- Sign in as an administrator or regular user, then log out to switch accounts.
- Create, view, edit, and delete maintenance requests, with role-based limits.
- Search requests and filter by status, priority, and category.
- Track request status, priority, assigned technician, and administrator notes.
- View dashboard statistics and export the currently displayed requests to CSV.
- Switch between English and Arabic and between light and dark themes.
- Save application data and user preferences locally as JSON files.

## Technology and structure

- **Language:** C#
- **User interface:** Windows Forms
- **Target framework:** .NET 8 for Windows
- **Storage:** Local JSON files; no external database or NuGet package is required.

```text
MaintenanceRequestManager.sln
├── MaintenanceRequestManager/
│   ├── Forms/       Windows Forms screens
│   ├── Models/      Users, requests, preferences, and app data
│   ├── Services/    Authentication, data, settings, and theme logic
│   └── Program.cs   Application entry point
├── RUN_PROJECT.bat
└── BUILD_RELEASE.bat
```

## Requirements

- Windows 10 or Windows 11
- .NET 8 SDK
- Visual Studio 2022 with the **.NET desktop development** workload (if using Visual Studio)

## Run the application

### Visual Studio

1. Open `MaintenanceRequestManager.sln`.
2. Allow Visual Studio to restore project assets.
3. Select `MaintenanceRequestManager` as the startup project if needed.
4. Press **F5** or click **Start**.

### Windows command prompt

Double-click `RUN_PROJECT.bat` from the project folder.

### PowerShell

Run these commands from the folder containing `MaintenanceRequestManager.sln`:

```powershell
dotnet restore
dotnet build
dotnet run --project .\MaintenanceRequestManager\MaintenanceRequestManager.csproj
```

## Demo accounts

| Role | Username | Password |
| --- | --- | --- |
| Administrator | `admin` | `admin123` |
| Regular user | `user` | `user123` |

These are sample accounts for demonstration. The application is an educational local desktop project and is not intended for production authentication or sensitive information.

## Data and preferences

The application creates its files under the current Windows user's local application data folder:

- Requests and users: `%LOCALAPPDATA%\MaintenanceRequestManager\data.json`
- Preferences: `%LOCALAPPDATA%\MaintenanceRequestManager\settings.json`

The files are created automatically when the application runs. Removing them resets the corresponding local data and preferences.

## Build a release

Double-click `BUILD_RELEASE.bat`, or run:

```powershell
dotnet publish .\MaintenanceRequestManager\MaintenanceRequestManager.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

The publish output is written under `MaintenanceRequestManager\bin\Release\net8.0-windows\win-x64\publish`.

## Project notes

The application uses local JSON persistence and sample credentials for coursework demonstration. A production version would require stronger authentication, protected credential storage, access controls, and a shared database.

See [`MaintenanceRequestManager/CHANGELOG.md`](MaintenanceRequestManager/CHANGELOG.md) for the project update history.
