$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln --no-restore
dotnet test OpenAPU.sln --no-build

Write-Host ""
Write-Host "OpenAPU 1.1.0 verificado."
