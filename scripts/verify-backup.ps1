param(
    [Parameter(Mandatory)]
    [string] $BackupPath
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $BackupPath)) {
    throw "No se encontrÃ³ el respaldo: $BackupPath"
}

$checksumPath = "$BackupPath.sha256"

if (-not (Test-Path $checksumPath)) {
    throw "No se encontrÃ³ el checksum: $checksumPath"
}

$expected = (
    Get-Content $checksumPath -Raw
).Trim().Split()[0].ToLower()

$actual = (
    Get-FileHash `
        -Path $BackupPath `
        -Algorithm SHA256
).Hash.ToLower()

if ($expected -ne $actual) {
    throw "El checksum SHA-256 no coincide."
}

$bytes = [System.IO.File]::ReadAllBytes($BackupPath)
$header = [System.Text.Encoding]::ASCII.GetString(
    $bytes,
    0,
    [Math]::Min(16, $bytes.Length))

if (-not $header.StartsWith("SQLite format 3")) {
    throw "La cabecera SQLite no es vÃ¡lida."
}

Write-Host ""
Write-Host "Respaldo Ã­ntegro."
Write-Host "Archivo: $BackupPath"
Write-Host "SHA-256: $actual"
