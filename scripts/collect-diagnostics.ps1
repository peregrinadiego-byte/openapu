param(
    [int] $Port = 8080,
    [string] $OutputDirectory = ""
)

$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $root "diagnostics"
}

New-Item -ItemType Directory -Force `
    $OutputDirectory | Out-Null

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$outputPath = Join-Path `
    $OutputDirectory `
    "openapu-diagnostics-$timestamp.json"

Invoke-WebRequest `
    -Uri "http://localhost:$Port/support/diagnostics/download" `
    -OutFile $outputPath

Write-Host ""
Write-Host "DiagnÃ³stico guardado en:"
Write-Host $outputPath
