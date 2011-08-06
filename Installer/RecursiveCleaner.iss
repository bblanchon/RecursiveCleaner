; RecursiveCleaner - Deletes files or folders according to filters defined in XML files.
; Copyright (C) 2011 Benoit Blanchon
;
; This program is free software: you can redistribute it and/or modify
; it under the terms of the GNU General Public License as published by
; the Free Software Foundation, either version 3 of the License, or
; (at your option) any later version.
; 
; This program is distributed in the hope that it will be useful,
; but WITHOUT ANY WARRANTY; without even the implied warranty of
; MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
; GNU General Public License for more details.
;
; You should have received a copy of the GNU General Public License
; along with this program.  If not, see <http://www.gnu.org/licenses/>

#define MyAppName "Recursive Cleaner"
#define MyAppVersion "0.1"
#define MyAppPublisher "Benoit Blanchon"
#define MyAppURL "https://code.google.com/p/recursive-cleaner/"
#define MyAppExeName "RecursiveCleaner.exe"
#define TaskName "RecursiveCleaner"
#define EventSource "RecursiveCleaner"

[Setup]
AppId={{6A79A916-21CE-4392-A86F-58BF0EE30A15}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=SetupRecursiveCleaner
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\RecursiveCleaner\bin\Release\RecursiveCleaner.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}";

[Tasks]
Name: schtasks; Description: "Create a scheduled task";

[Run]
StatusMsg: "Creating event source..."; Filename: "{sys}\eventcreate.exe"; Parameters:"/ID 1 /L APPLICATION /T INFORMATION /SO ""{#EventSource}"" /D ""{#MyAppName} installed"""; Flags: runhidden runascurrentuser;
StatusMsg: "Deleting existing task..."; Filename: "{sys}\schtasks.exe"; Parameters:"/Delete /F /TN ""{#TaskName}"""; Flags: runhidden runascurrentuser; Tasks: schtasks;
StatusMsg: "Creating scheduled task..."; Filename: "{sys}\schtasks.exe"; Parameters:"/Create /RU SYSTEM /SC ONIDLE /I 15 /TN ""{#TaskName}"" /TR ""{app}\{#MyAppExeName} -a -e"""; Flags: runascurrentuser runhidden; Tasks: schtasks;

[UninstallRun]
StatusMsg: "Deleting scheduled task..."; Filename: "{sys}\schtasks.exe"; Parameters:"/Delete /F /TN ""{#TaskName}"""; Flags: runhidden runascurrentuser; Tasks: schtasks;

