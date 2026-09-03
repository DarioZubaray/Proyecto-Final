Get-ChildItem -Path $PSScriptRoot -Filter *.mmd | ForEach-Object {
    $output = Join-Path $_.Directory "$($_.BaseName).png"
    Write-Host "Generando $($_.BaseName).png ..."
    mmdc -i $_.FullName -o $output
}
Write-Host "Listo."
