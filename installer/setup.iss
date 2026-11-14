; ============================================================
; Trabajo Final - Dario Zubaray
; Instalador per-user (sin UAC) con Inno Setup 6
;
; Compilar (desde installer\):
;   ISCC.exe /DMode=offline setup.iss   -> Output\Setup-TrabajoFinal-offline.exe
;   ISCC.exe /DMode=online  setup.iss   -> Output\Setup-TrabajoFinal-online.exe
;
; Modos:
;   offline = self-contained (no requiere .NET instalado)
;   online  = framework-dependent (descarga .NET 10 Desktop Runtime
;             a %LocalAppData%\Microsoft\dotnet y fija DOTNET_ROOT)
;
; Flujo: wizard (conexion) -> copia de archivos -> escribe
; TrabajoFinal-DarioZubaray.dll.config -> --bootstrap (crea la BD
; si falta y aplica 01 -> 06) -> --test-connection -> finish.
; ============================================================

#ifndef Mode
  #define Mode "offline"
#endif

#define AppName "TrabajoFinal - Dario Zubaray"
#define AppExeName "TrabajoFinal-DarioZubaray.exe"
#define AppIdGuid "{{8F3C2A71-5B9E-4D28-9C6F-1A4E7B0D2E55}"

[Setup]
AppId={#AppIdGuid}
AppName={#AppName}
AppVersion=1.0.0
AppPublisher=Dario Zubaray
VersionInfoVersion=1.0.0.0
DefaultDirName={localappdata}\Programs\TrabajoFinal-DarioZubaray
DefaultGroupName=TrabajoFinal - Dario Zubaray
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
; Desde Inno 5.5.7 la bienvenida viene deshabilitada por defecto (guia Microsoft).
DisableWelcomePage=no
OutputDir=Output
OutputBaseFilename=Setup-TrabajoFinal-{#Mode}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
SetupIconFile=..\src\TrabajoFinal-DarioZubaray\GUI\DarioZubaray_TF.ico

[Messages]
; Título grande de la página de bienvenida
WelcomeLabel1=Bienvenido al asistente de instalación de [name/ver]
; Descripción genérica de lo que el asistente hará
WelcomeLabel2=Este asistente instalará [name] en su equipo.%n%nDurante la instalación se copiarán los archivos de la aplicación, se configurará la conexión a Microsoft SQL Server y se preparará la base de datos con sus scripts de esquema.%n%nSe recomienda cerrar las demás aplicaciones antes de continuar.

[Languages]
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
#if Mode == "online"
Source: "publish\online\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "sql\*"
#else
Source: "publish\offline\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "sql\*"
#endif
Source: "..\src\sql\01_*.sql"; DestDir: "{app}\sql"; Flags: ignoreversion
Source: "..\src\sql\06_*.sql"; DestDir: "{app}\sql"; Flags: ignoreversion
;Source: "..\src\sql\03_*.sql"; DestDir: "{app}\sql"; Flags: ignoreversion
;Source: "..\src\sql\04_*.sql"; DestDir: "{app}\sql"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

#if Mode == "online"
[Registry]
Root: HKCU; Subkey: "Environment"; ValueType: string; ValueName: "DOTNET_ROOT"; \
  ValueData: "{localappdata}\Microsoft\dotnet"; Flags: uninsdeletevalue
#endif

[Run]
Filename: "{code:LaunchFilename}"; Parameters: "{code:LaunchParams}"; \
  Description: "{cm:LaunchProgram,{#AppName}}"; \
  Flags: postinstall nowait skipifsilent; Check: BootstrapOk

[InstallDelete]
; Limpieza total antes de copiar: evita restos de una version
; anterior (por ejemplo al cambiar entre modo offline y online).
Type: filesandordirs; Name: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
var
  ConnPage: TInputQueryWizardPage;
  GBootstrapOk: Boolean;

function XmlEscape(const S: String): String;
var
  T: String;
begin
  T := S;
  StringChangeEx(T, '&', '&amp;', True);
  StringChangeEx(T, '<', '&lt;', True);
  StringChangeEx(T, '>', '&gt;', True);
  StringChangeEx(T, '"', '&quot;', True);
  Result := T;
end;

function AppExePath: String;
begin
  Result := ExpandConstant('{app}\{#AppExeName}');
end;

{ Ejecuta la CLI de la aplicacion. En modo online el exe es
  framework-dependent: se le inyecta DOTNET_ROOT via cmd para que
  no dependa del entorno heredado del instalador. }
function RunAppCli(const Args: String; var ResultCode: Integer): Boolean;
var
  Params: String;
begin
#if Mode == "online"
  Params := '/C set "DOTNET_ROOT=' + ExpandConstant('{localappdata}\Microsoft\dotnet') +
            '" && "' + AppExePath + '" ' + Args;
  Result := Exec(ExpandConstant('{cmd}'), Params, '', SW_HIDE,
                 ewWaitUntilTerminated, ResultCode);
#else
  Result := Exec(AppExePath, Args, '', SW_HIDE,
                 ewWaitUntilTerminated, ResultCode);
#endif
end;

function BootstrapOk: Boolean;
begin
  Result := GBootstrapOk;
end;

function LaunchFilename(Param: String): String;
begin
#if Mode == "online"
  Result := ExpandConstant('{cmd}');
#else
  Result := AppExePath;
#endif
end;

function LaunchParams(Param: String): String;
begin
#if Mode == "online"
  Result := '/C set "DOTNET_ROOT=' + ExpandConstant('{localappdata}\Microsoft\dotnet') +
            '" && "' + AppExePath + '"';
#else
  Result := '';
#endif
end;

procedure InitializeWizard;
begin
  GBootstrapOk := False;
  ConnPage := CreateInputQueryPage(wpWelcome,
    'Conexion a la base de datos',
    'Ingrese los datos de conexion a Microsoft SQL Server.',
    'La aplicacion usara estos valores para conectarse. ' +
    'Si la base de datos no existe, se creara en la instalacion.');
  ConnPage.Add('Servidor:', False);
  ConnPage.Add('Base de datos:', False);
  ConnPage.Values[0] := 'localhost\SQLEXPRESS';
  ConnPage.Values[1] := 'DarioZubaray_TF';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID = ConnPage.ID then begin
    if Trim(ConnPage.Values[0]) = '' then begin
      MsgBox('Debe indicar el servidor SQL Server.', mbError, MB_OK);
      Result := False;
    end else if Trim(ConnPage.Values[1]) = '' then begin
      MsgBox('Debe indicar el nombre de la base de datos.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;

procedure WriteAppConfig;
var
  CfgPath, Content: String;
begin
  CfgPath := ExpandConstant('{app}\TrabajoFinal-DarioZubaray.dll.config');
  Content :=
    '<?xml version="1.0" encoding="utf-8" ?>' + #13#10 +
    '<configuration>' + #13#10 +
    '  <connectionStrings>' + #13#10 +
    '    <add name="cadenaConexion"' + #13#10 +
    '         connectionString="Server=' + XmlEscape(Trim(ConnPage.Values[0])) +
    ';Database=' + XmlEscape(Trim(ConnPage.Values[1])) +
    ';Integrated Security=True;Encrypt=False;" />' + #13#10 +
    '  </connectionStrings>' + #13#10 +
    '</configuration>' + #13#10;
  if not SaveStringToFile(CfgPath, Content, False) then
    RaiseException('No se pudo escribir el archivo de configuracion.');
end;

#if Mode == "online"
procedure InstallDesktopRuntime;
var
  Ps1Path, InstallDir: String;
  ResultCode: Integer;
begin
  Ps1Path := ExpandConstant('{tmp}\dotnet-install.ps1');
  DownloadTemporaryFile('https://dot.net/v1/dotnet-install.ps1', 'dotnet-install.ps1', '', nil);
  InstallDir := ExpandConstant('{localappdata}\Microsoft\dotnet');

  { Runtime base: hostfxr + Microsoft.NETCore.App (lo necesita el apphost) }
  if not Exec('powershell.exe',
      '-NoProfile -ExecutionPolicy Bypass -File "' + Ps1Path + '"' +
      ' -Channel 10.0 -Runtime dotnet -InstallDir "' + InstallDir + '"',
      '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    RaiseException('No se pudo ejecutar la descarga del runtime base de .NET.');
  if ResultCode <> 0 then
    RaiseException('La instalacion del runtime base de .NET fallo (codigo ' +
      IntToStr(ResultCode) + '). Verifique su conexion a internet.');

  { Windows Desktop: Microsoft.WindowsDesktop.App (WinForms) }
  if not Exec('powershell.exe',
      '-NoProfile -ExecutionPolicy Bypass -File "' + Ps1Path + '"' +
      ' -Channel 10.0 -Runtime windowsdesktop -InstallDir "' + InstallDir + '"',
      '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    RaiseException('No se pudo ejecutar la descarga de .NET Desktop.');
  if ResultCode <> 0 then
    RaiseException('La instalacion de .NET 10 Desktop Runtime fallo (codigo ' +
      IntToStr(ResultCode) + '). Verifique su conexion a internet.');
end;
#endif

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  ErrMsg: String;
begin
  if CurStep = ssPostInstall then begin
    try
      Log('Instalacion: escribiendo configuracion...');
      WriteAppConfig;
      Log('Instalacion: configuracion escrita.');

#if Mode == "online"
      Log('Instalacion: instalando .NET Desktop Runtime...');
      InstallDesktopRuntime;
      Log('Instalacion: runtime listo.');
      { setx escribe el valor y difunde WM_SETTINGCHANGE para que
        el Explorador lo vea (atajos del menu de inicio). }
      Exec('setx.exe', 'DOTNET_ROOT "' + ExpandConstant('{localappdata}\Microsoft\dotnet') + '"',
        '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
#endif

      Log('Instalacion: ejecutando --bootstrap...');
      if not RunAppCli('--bootstrap', ResultCode) then
        RaiseException('No se pudo ejecutar la preparacion de la base de datos.');
      if ResultCode <> 0 then
        RaiseException('La preparacion de la base de datos fallo (codigo ' +
          IntToStr(ResultCode) + '). Detalle en el archivo ' +
          'TrabajoFinal-DarioZubaray_bootstrap.log en la carpeta TEMP de Windows.');
      Log('Instalacion: bootstrap OK.');

      if not RunAppCli('--test-connection', ResultCode) then
        RaiseException('No se pudo verificar la conexion a la base de datos.');
      if ResultCode <> 0 then
        RaiseException('La verificacion de conexion fallo (codigo ' +
          IntToStr(ResultCode) + '). Detalle en el archivo ' +
          'TrabajoFinal-DarioZubaray_bootstrap.log en la carpeta TEMP de Windows.');
      Log('Instalacion: conexion OK.');

      GBootstrapOk := True;
    except
      GBootstrapOk := False;
      ErrMsg := GetExceptionMessage;
      Log('Instalacion ERROR: ' + ErrMsg);
      SaveStringToFile(ExpandConstant('{localappdata}\TrabajoFinal-instalacion.log'),
        ErrMsg + #13#10, True);
      MsgBox(ErrMsg, mbError, MB_OK);
    end;
  end;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then begin
    if GBootstrapOk then
      WizardForm.FinishedLabel.Caption :=
        'La instalacion finalizo correctamente. La base de datos esta lista para usar.'
    else
      WizardForm.FinishedLabel.Caption :=
        'La instalacion copio los archivos, pero la base de datos no pudo prepararse. ' +
        'Revise el detalle en TrabajoFinal-DarioZubaray_bootstrap.log (TEMP) y ' +
        'vuelva a ejecutar la aplicacion para reintentar la conexion.';
  end;
end;
