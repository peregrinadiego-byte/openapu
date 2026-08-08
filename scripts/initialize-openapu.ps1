param(
    [int] $Port = 8080,
    [switch] $WithoutAdminKey
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$root = Split-Path $PSScriptRoot -Parent
Set-Location $root

function New-SecureAdminKey {
    $bytes = New-Object byte[] 32

    [System.Security.Cryptography.RandomNumberGenerator]::Fill(
        $bytes)

    return [Convert]::ToHexString($bytes).ToLowerInvariant()
}

try {
    docker info | Out-Null
}
catch {
    throw "Docker no está disponible. Inicia Docker Desktop y vuelve a ejecutar este script."
}

$envPath = Join-Path $root ".env"

if (-not (Test-Path $envPath)) {
    $lines = @(
        "OPENAPU_PORT=$Port"
    )

    if ($WithoutAdminKey) {
        $lines += "OPENAPU_ADMIN_KEY="
    }
    else {
        $adminKey = New-SecureAdminKey
        $lines += "OPENAPU_ADMIN_KEY=$adminKey"
    }

    $lines |
        Set-Content `
            -Path $envPath `
            -Encoding UTF8

    Write-Host ".env creado."
}
else {
    Write-Host ".env existente conservado."
}

docker compose up -d --build

$deadline = (Get-Date).AddSeconds(90)
$ready = $false

do {
    try {
        $status = Invoke-RestMethod `
            -Uri "http://localhost:$Port/ready" `
            -TimeoutSec 5

        if ($status.ready -eq $true) {
            $ready = $true
            break
        }
    }
    catch {
        Start-Sleep -Seconds 2
    }
}
while ((Get-Date) -lt $deadline)

if (-not $ready) {
    docker compose logs --tail 100
    throw "OpenAPU no alcanzó estado preparado."
}

$adminKeyValue = ""

$envLines = Get-Content $envPath

foreach ($line in $envLines) {
    if ($line.StartsWith("OPENAPU_ADMIN_KEY=")) {
        $adminKeyValue = $line.Substring(
            "OPENAPU_ADMIN_KEY=".Length)

        break
    }
}

if ([string]::IsNullOrWhiteSpace($adminKeyValue)) {
    & "$PSScriptRoot\smoke-test.ps1" `
        -Port $Port `
        -ExpectedVersion "1.4.0"
}
else {
    & "$PSScriptRoot\smoke-test.ps1" `
        -Port $Port `
        -ExpectedVersion "1.4.0" `
        -AdminKey $adminKeyValue
}

Write-Host ""
Write-Host "OpenAPU listo."
Write-Host "URL: http://localhost:$Port"
Write-Host "Configuración local: $envPath"

if (-not [string]::IsNullOrWhiteSpace($adminKeyValue)) {
    Write-Host "Protección administrativa: activa."
    Write-Host "La clave está guardada únicamente en .env."
}
else {
    Write-Host "Protección administrativa: desactivada."
}
