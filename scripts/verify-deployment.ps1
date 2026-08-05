param(
    [int] $Port = 8080,
    [int] $TimeoutSeconds = 60
)

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:$Port"
$deadline = (Get-Date).AddSeconds($TimeoutSeconds)
$ready = $false

Write-Host "Esperando OpenAPU en $baseUrl ..."

do {
    try {
        $health = Invoke-RestMethod `
            -Uri "$baseUrl/health" `
            -Method Get `
            -TimeoutSec 5

        if ($health.status -eq "ok") {
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
    throw "OpenAPU no respondiÃ³ dentro de $TimeoutSeconds segundos."
}

$status = Invoke-RestMethod `
    -Uri "$baseUrl/system/status" `
    -Method Get `
    -TimeoutSec 10

if ($status.name -ne "OpenAPU") {
    throw "La instancia no se identificÃ³ como OpenAPU."
}

if ($status.database -ne "ready") {
    throw "La base de datos no estÃ¡ lista."
}

Write-Host ""
Write-Host "OpenAPU disponible."
Write-Host "VersiÃ³n: $($status.version)"
Write-Host "Base de datos: $($status.database)"
Write-Host "URL: $baseUrl"
