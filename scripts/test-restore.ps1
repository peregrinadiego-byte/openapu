param(
    [int] $SourcePort = 8080,
    [int] $TestPort = 8082,
    [string] $Image = "openapu:1.4.0",
    [string] $AdminKey = $env:OPENAPU_ADMIN_KEY
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

$root = Split-Path $PSScriptRoot -Parent
$backupDirectory = Join-Path $root "backups"
$volumeName = "openapu-restore-test"
$containerName = "openapu-restore-test"

New-Item -ItemType Directory -Force `
    $backupDirectory | Out-Null

& "$PSScriptRoot\backup-openapu.ps1" `
    -Port $SourcePort `
    -OutputDirectory $backupDirectory `
    -AdminKey $AdminKey

$backup = Get-ChildItem `
    -Path $backupDirectory `
    -Filter "openapu-*.db" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $backup) {
    throw "No se encontrÃ³ un respaldo para probar."
}

& "$PSScriptRoot\verify-backup.ps1" `
    -BackupPath $backup.FullName

$existingContainer = docker ps -a `
    --filter "name=^/$containerName$" `
    --format "{{.Names}}"

if ($existingContainer -eq $containerName) {
    docker rm --force $containerName | Out-Null
}

$existingVolume = docker volume ls `
    --filter "name=^$volumeName$" `
    --format "{{.Name}}"

if ($existingVolume -eq $volumeName) {
    docker volume rm $volumeName | Out-Null
}

docker volume create $volumeName | Out-Null

try {
    $backupDirectoryForDocker = (
        Resolve-Path $backupDirectory
    ).Path

    docker run `
        --rm `
        --user 0 `
        --entrypoint sh `
        --volume "${volumeName}:/data" `
        --volume "${backupDirectoryForDocker}:/backup:ro" `
        $Image `
        -c "cp /backup/$($backup.Name) /data/openapu.db && chown 1654:1654 /data/openapu.db"

    docker run `
        --detach `
        --name $containerName `
        --publish "${TestPort}:8080" `
        --env ASPNETCORE_URLS=http://+:8080 `
        --env OPENAPU_DB_PATH=/data/openapu.db `
        --volume "${volumeName}:/data" `
        $Image | Out-Null

    $deadline = (Get-Date).AddSeconds(60)
    $ready = $false

    do {
        try {
            $status = Invoke-RestMethod `
                -Uri "http://localhost:$TestPort/ready" `
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
        docker logs $containerName
        throw "La instancia restaurada no alcanzÃ³ estado preparado."
    }

    $systemStatus = Invoke-RestMethod `
        -Uri "http://localhost:$TestPort/system/status" `
        -TimeoutSec 10

    if ($systemStatus.database -ne "ready") {
        throw "La base restaurada no quedÃ³ disponible."
    }

    Write-Host ""
    Write-Host "Prueba de restauraciÃ³n superada."
    Write-Host "Respaldo: $($backup.FullName)"
    Write-Host "VersiÃ³n: $($systemStatus.version)"
    Write-Host "Base de datos: $($systemStatus.database)"
}
finally {
    $container = docker ps -a `
        --filter "name=^/$containerName$" `
        --format "{{.Names}}"

    if ($container -eq $containerName) {
        docker rm --force $containerName | Out-Null
    }

    $volume = docker volume ls `
        --filter "name=^$volumeName$" `
        --format "{{.Name}}"

    if ($volume -eq $volumeName) {
        docker volume rm $volumeName | Out-Null
    }
}


