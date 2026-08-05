param(
    [switch] $SkipTests
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$root = Split-Path $PSScriptRoot -Parent
Set-Location $root

$forbiddenTracked = git ls-files |
    Where-Object {
        $_ -match '(^|/)(bin|obj|artifacts|backups|data|TestResults)/' -or
        $_ -match '\.(db|db-shm|db-wal|zip)$'
    }

if ($forbiddenTracked) {
    Write-Host "Archivos generados rastreados por Git:"
    $forbiddenTracked | ForEach-Object {
        Write-Host " - $_"
    }

    throw "El repositorio contiene artefactos locales rastreados."
}

$attributeCheck = git check-attr text eol -- README.md

if (-not $attributeCheck) {
    throw "No se pudieron comprobar los atributos Git."
}

dotnet format OpenAPU.sln --verify-no-changes --no-restore

if (-not $SkipTests) {
    dotnet restore OpenAPU.sln
    dotnet build OpenAPU.sln --configuration Release --no-restore
    dotnet test OpenAPU.sln --configuration Release --no-build
}

Write-Host ""
Write-Host "Repositorio OpenAPU verificado."
