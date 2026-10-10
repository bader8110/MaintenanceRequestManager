# Application Architecture

This diagram summarizes how the Windows Forms screens, application services, data models, and local files work together.

```mermaid
flowchart TB
    subgraph UI[Windows Forms UI]
        Program[Program startup]
        Login[Login form]
        Dashboard[Request dashboard]
        RequestForm[Create or edit request]
        Details[Request details]
        Settings[Settings form]
    end

    subgraph Services[Application services]
        Auth[AuthService]
        Data[DataService]
        Preferences[SettingsService]
        Theme[ThemeService]
    end

    subgraph Models[Application models]
        User[User and role]
        Ticket[Maintenance ticket]
        AppData[AppData]
        UserPrefs[User preferences]
    end

    subgraph Storage[Local files]
        DataJson[data.json]
        SettingsJson[settings.json]
        Csv[Exported CSV]
    end

    Program --> Login
    Login --> Auth
    Auth --> Data
    Auth --> User
    Login -->|Successful login| Dashboard
    Dashboard --> Data
    Dashboard -->|New or edit| RequestForm
    Dashboard -->|Open selected item| Details
    Dashboard --> Settings
    RequestForm --> Data
    Details --> Data
    Settings --> Preferences
    Settings --> Theme
    Dashboard --> Theme
    Data --> AppData
    AppData --> User
    AppData --> Ticket
    Data -->|Read and write| DataJson
    Preferences --> UserPrefs
    Preferences -->|Read and write| SettingsJson
    Dashboard -->|Export displayed requests| Csv
```

## Request lifecycle

```mermaid
flowchart TD
    Start([Start application]) --> SignIn[Sign in]
    SignIn --> Valid{Credentials valid?}
    Valid -->|No| SignIn
    Valid -->|Yes| Dashboard[Open dashboard]
    Dashboard --> Action{Choose an action}
    Action -->|Create request| Create[Enter request details]
    Action -->|Search or filter| Filter[Show matching requests]
    Action -->|Manage selected request| Manage[View, edit, update, or delete]
    Action -->|Change preferences| Pref[Choose language or theme]
    Action -->|Export| Export[Save displayed requests as CSV]
    Create --> Save[Save changes to local JSON]
    Manage --> Save
    Pref --> SavePref[Save preferences to local JSON]
    Save --> Dashboard
    SavePref --> Dashboard
    Filter --> Dashboard
    Export --> Dashboard
    Dashboard --> Logout[Log out]
    Logout --> SignIn
```

## Storage

- Request and account data are stored in `%LOCALAPPDATA%\MaintenanceRequestManager\data.json`.
- Language and appearance preferences are stored in `%LOCALAPPDATA%\MaintenanceRequestManager\settings.json`.
- CSV export writes the currently displayed request list to a file selected by the application.

The application uses local files and sample coursework credentials. It is not designed for production use or sensitive data.
