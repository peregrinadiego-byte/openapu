$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$output = Join-Path $PSScriptRoot "..\backups\openapu-$timestamp.db"

New-Item -ItemType Directory -Force `
    (Split-Path $output) | Out-Null

Invoke-WebRequest `
    -Uri "http://localhost:8080/database/backup" `
    -OutFile $output

Write-Host "Respaldo guardado en:"
Write-Host (Resolve-Path $output)
