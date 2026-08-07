param(
    [int] $Port = 8080,
    [string] $OutputDirectory = "",
    [string] $AdminKey = $env:OPENAPU_ADMIN_KEY
)

$ErrorActionPreference = "Stop"

$root = Split-Path $PSScriptRoot -Parent

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $root "backups"
}

New-Item -ItemType Directory -Force `
    $OutputDirectory | Out-Null

$headers = @{}

if (-not [string]::IsNullOrWhiteSpace($AdminKey)) {
    $headers["X-OpenAPU-Admin-Key"] = $AdminKey
}

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupPath = Join-Path `
    $OutputDirectory `
    "openapu-$timestamp.db"

$checksumPath = "$backupPath.sha256"

Invoke-WebRequest `
    -Uri "http://localhost:$Port/database/backup" `
    -Headers $headers `
    -OutFile $backupPath

if (-not (Test-Path $backupPath)) {
    throw "El respaldo no fue creado."
}

$length = (Get-Item $backupPath).Length

if ($length -lt 100) {
    throw "El respaldo es demasiado pequeño para ser válido."
}

$bytes = [System.IO.File]::ReadAllBytes($backupPath)
$header = [System.Text.Encoding]::ASCII.GetString(
    $bytes,
    0,
    [Math]::Min(16, $bytes.Length))

if (-not $header.StartsWith("SQLite format 3")) {
    throw "El archivo no contiene una cabecera SQLite válida."
}

$hash = Get-FileHash `
    -Path $backupPath `
    -Algorithm SHA256

"$($hash.Hash.ToLower())  $([System.IO.Path]::GetFileName($backupPath))" |
    Set-Content $checksumPath -Encoding ASCII

Write-Host ""
Write-Host "Respaldo creado y validado."
Write-Host "Archivo: $backupPath"
Write-Host "SHA-256: $($hash.Hash.ToLower())"
Write-Host "Checksum: $checksumPath"
