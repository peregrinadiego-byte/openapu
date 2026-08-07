param(
    [int] $Port = 8080,
    [string] $OutputDirectory = "",
    [string] $AdminKey = $env:OPENAPU_ADMIN_KEY
)

$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $root "diagnostics"
}

New-Item -ItemType Directory -Force `
    $OutputDirectory | Out-Null

$headers = @{}

if (-not [string]::IsNullOrWhiteSpace($AdminKey)) {
    $headers["X-OpenAPU-Admin-Key"] = $AdminKey
}

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$outputPath = Join-Path `
    $OutputDirectory `
    "openapu-diagnostics-$timestamp.json"

Invoke-WebRequest `
    -Uri "http://localhost:$Port/support/diagnostics/download" `
    -Headers $headers `
    -OutFile $outputPath

Write-Host ""
Write-Host "Diagnóstico guardado en:"
Write-Host $outputPath
