<#
.SYNOPSIS
  Construye los instaladores finales (.exe) de TrabajoFinal-DarioZubaray.

.DESCRIPTION
  Pipeline:
    1. dotnet publish  -> installer\publish\{offline|online}\  (binarios)
    2. ISCC setup.iss   -> installer\Output\Setup-TrabajoFinal-{modo}.exe

  El SQL de src\sql se empaqueta directamente desde el repositorio
  (setup.iss lo referencia), por lo que un cambio solo de SQL o solo
  de setup.iss no requiere volver a publicar.

.PARAMETER Mode
  offline | online | all (por defecto all)

.PARAMETER SkipPublish
  Omite dotnet publish y solo recompila setup.iss con ISCC
  (para cambios solo en SQL o en el propio .iss).

.EXAMPLE
  .\build.ps1                  # publica y compila ambos modos
  .\build.ps1 -Mode offline    # solo el instalador offline
  .\build.ps1 -SkipPublish     # solo ISCC (SQL/iss cambiaron)
#>
param(
  [ValidateSet('offline', 'online', 'all')]
  [string]$Mode = 'all',
  [switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'
$installerDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $installerDir '..\src\TrabajoFinal-DarioZubaray\GUI\GUI.csproj'
$iss = Join-Path $installerDir 'setup.iss'

function Find-ISCC {
  $candidates = @(
    (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe'),
    (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
    (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe')
  )
  foreach ($c in $candidates) {
    if ($c -and (Test-Path $c)) { return $c }
  }
  $cmd = Get-Command 'ISCC.exe' -ErrorAction SilentlyContinue
  if ($cmd) { return $cmd.Source }
  throw 'No se encontro ISCC.exe (Inno Setup 6).'
}

$iscc = Find-ISCC
$modes = if ($Mode -eq 'all') { @('offline', 'online') } else { @($Mode) }

foreach ($m in $modes) {
  $publishDir = Join-Path $installerDir "publish\$m"

  if (-not $SkipPublish) {
    $selfContained = if ($m -eq 'offline') { 'true' } else { 'false' }
    Write-Host "==> dotnet publish ($m, self-contained=$selfContained)" -ForegroundColor Cyan
    if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
    dotnet publish $project -c Release -r win-x64 --self-contained $selfContained -o $publishDir --nologo
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish fallo (exit $LASTEXITCODE) en modo $m." }
  } else {
    if (-not (Test-Path $publishDir)) {
      throw "No existe $publishDir. Ejecuta sin -SkipPublish primero."
    }
  }

  Write-Host "==> ISCC /DMode=$m" -ForegroundColor Cyan
  & $iscc "/DMode=$m" $iss
  if ($LASTEXITCODE -ne 0) { throw "ISCC fallo (exit $LASTEXITCODE) en modo $m." }
}

Write-Host ''
Write-Host 'Instaladores generados:' -ForegroundColor Green
Get-ChildItem (Join-Path $installerDir 'Output\*.exe') |
  Select-Object Name, @{n = 'MB'; e = { [math]::Round($_.Length / 1MB, 1) } }, LastWriteTime |
  Format-Table -AutoSize
