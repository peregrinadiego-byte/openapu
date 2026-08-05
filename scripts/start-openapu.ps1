$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

docker compose up -d --build

& "$PSScriptRoot\verify-deployment.ps1"

Write-Host ""
Write-Host "OpenAPU iniciado."
Write-Host "Interfaz: http://localhost:8080"
