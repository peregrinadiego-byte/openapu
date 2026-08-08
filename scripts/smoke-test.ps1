param(
    [int] $Port = 8080,
    [string] $ExpectedVersion = "1.4.0",
    [string] $AdminKey = $env:OPENAPU_ADMIN_KEY
)

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:$Port"

function Assert-Equal {
    param(
        [object] $Actual,
        [object] $Expected,
        [string] $Message
    )

    if ($Actual -ne $Expected) {
        throw "$Message Esperado: '$Expected'. Actual: '$Actual'."
    }
}

function Assert-True {
    param(
        [bool] $Condition,
        [string] $Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

Write-Host "Probando OpenAPU en $baseUrl ..."

$rootStatus = Invoke-RestMethod `
    -Uri "$baseUrl/" `
    -Method Get `
    -TimeoutSec 10

Assert-Equal `
    $rootStatus.name `
    "OpenAPU" `
    "Identidad de producto incorrecta."

Assert-Equal `
    $rootStatus.version `
    $ExpectedVersion `
    "Versión inesperada."

Assert-Equal `
    $rootStatus.status `
    "ready" `
    "El endpoint raíz no está preparado."

$health = Invoke-RestMethod `
    -Uri "$baseUrl/health" `
    -Method Get `
    -TimeoutSec 10

Assert-Equal `
    $health.status `
    "ok" `
    "Health check incorrecto."

$ready = Invoke-RestMethod `
    -Uri "$baseUrl/ready" `
    -Method Get `
    -TimeoutSec 10

Assert-True `
    ($ready.ready -eq $true) `
    "El endpoint /ready no reporta preparación."

$system = Invoke-RestMethod `
    -Uri "$baseUrl/system/status" `
    -Method Get `
    -TimeoutSec 10

Assert-Equal `
    $system.database `
    "ready" `
    "La base de datos no está preparada."

$ui = Invoke-WebRequest `
    -Uri "$baseUrl/index.html" `
    -Method Get `
    -TimeoutSec 10

Assert-Equal `
    $ui.StatusCode `
    200 `
    "La interfaz web no respondió correctamente."

Assert-True `
    ($ui.Content -match "OpenAPU") `
    "La interfaz no contiene la identidad OpenAPU."

if ([string]::IsNullOrWhiteSpace($AdminKey)) {
    Write-Host "Protección administrativa: desactivada."
}
else {
    try {
        Invoke-WebRequest `
            -Uri "$baseUrl/support/diagnostics" `
            -Method Get `
            -TimeoutSec 10 `
            -ErrorAction Stop | Out-Null

        throw "El diagnóstico administrativo aceptó una solicitud sin clave."
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__

        if ($statusCode -ne 401) {
            throw
        }
    }

    $diagnostics = Invoke-RestMethod `
        -Uri "$baseUrl/support/diagnostics" `
        -Method Get `
        -Headers @{
            "X-OpenAPU-Admin-Key" = $AdminKey
        } `
        -TimeoutSec 10

    Assert-Equal `
        $diagnostics.product `
        "OpenAPU" `
        "Diagnóstico administrativo incorrecto."

    Write-Host "Protección administrativa: validada."
}

Write-Host ""
Write-Host "Smoke test superado."
Write-Host "Versión: $ExpectedVersion"
Write-Host "Base de datos: ready"
Write-Host "Interfaz: disponible"
