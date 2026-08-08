param(
    [string] $Version = "1.4.0",
    [ValidateSet("win-x64", "linux-x64")]
    [string[]] $Runtime = @("win-x64", "linux-x64")
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$root = Split-Path $PSScriptRoot -Parent
Set-Location $root

$outputRoot = Join-Path $root "artifacts"
Remove-Item $outputRoot -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $outputRoot | Out-Null

dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln --configuration Release --no-restore
dotnet test OpenAPU.sln --configuration Release --no-build

foreach ($target in $Runtime) {
    $publishPath = Join-Path $outputRoot $target
    $archivePath = Join-Path `
        $outputRoot `
        "OpenAPU-v$Version-$target.zip"

    dotnet publish `
        src\OpenAPU.Api\OpenAPU.Api.csproj `
        --configuration Release `
        --runtime $target `
        --self-contained false `
        -p:Version=$Version `
        --output $publishPath

    Copy-Item README.md $publishPath
    Copy-Item RELEASE_NOTES.md $publishPath
    Copy-Item compose.yaml $publishPath

    Compress-Archive `
        -Path "$publishPath\*" `
        -DestinationPath $archivePath `
        -Force

    Write-Host "Paquete creado: $archivePath"
}

Write-Host ""
Write-Host "Paquetes de OpenAPU v$Version generados."




