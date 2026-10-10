#define AppName "Inventory Management"
#define AppVersion GetEnv("APP_VERSION")
#define AppExeName "InventoryApp.exe"

[Setup]
AppId={{5A45D0D4-CE1B-4F3C-9B9B-2B0A2C2A8A71}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher="MAUI-Project"
DefaultDirName={localappdata}\Programs\InventoryApp
DefaultGroupName={#AppName}
OutputDir=..\artifacts\release
OutputBaseFilename=InventoryApp-Setup-v{#AppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
CloseApplications=yes
RestartApplications=yes
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "..\artifacts\windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: postinstall nowait skipifsilent
