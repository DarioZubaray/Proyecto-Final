$config = Join-Path $PSScriptRoot "puppeteer-config.json"
$excluir = @("4.1.diagrama-de-casos-de-uso")
$extraArgs = @{
    "7.clases" = "-w 2600"
}

Get-ChildItem -Path $PSScriptRoot -Filter *.mmd | ForEach-Object {
    if ($excluir -contains $_.BaseName) {
        Write-Host "Omitiendo $($_.BaseName).mmd (fuera del script)"
        return
    }
    $output = Join-Path $_.Directory "$($_.BaseName).png"
    Write-Host "Generando $($_.BaseName).png ..."
    try {
        if ($extraArgs.ContainsKey($_.BaseName)) {
            mmdc -i $_.FullName -o $output -p $config -s 3 $($extraArgs[$_.BaseName] -split ' ')
        }
        else {
            mmdc -i $_.FullName -o $output -p $config -s 3
        }
        if ($LASTEXITCODE -ne 0) { Write-Host "Error al generar $($_.BaseName).png" }
    }
    catch {
        Write-Host "Error generando $($_.BaseName).png: $($_.Exception.Message)"
    }
}
Write-Host "Listo."
