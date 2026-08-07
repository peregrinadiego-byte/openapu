param(
    [int] $Port = 8080,
    [int] $Keep = 10,
    [string] $AdminKey = $env:OPENAPU_ADMIN_KEY
)

$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent
$backupDirectory = Join-Path $root "backups"

& "$PSScriptRoot\backup-openapu.ps1" `
    -Port $Port `
    -OutputDirectory $backupDirectory `
    -AdminKey $AdminKey

$backups = Get-ChildItem `
    -Path $backupDirectory `
    -Filter "openapu-*.db" |
    Sort-Object LastWriteTime -Descending

$obsolete = $backups | Select-Object -Skip $Keep

foreach ($file in $obsolete) {
    Remove-Item $file.FullName -Force

    $checksum = "$($file.FullName).sha256"

    if (Test-Path $checksum) {
        Remove-Item $checksum -Force
    }
}

Write-Host ""
Write-Host "Retención aplicada."
Write-Host "Respaldos conservados: $Keep"
