$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

docker compose down

Write-Host "OpenAPU detenido."
Write-Host "El volumen openapu-data se conserva."
