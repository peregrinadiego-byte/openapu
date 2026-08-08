# Administrative access key

OpenAPU can protect sensitive administrative endpoints with a shared key.

Environment variable:

```text
OPENAPU_ADMIN_KEY
```

Request header:

```text
X-OpenAPU-Admin-Key
```

Protected routes:

```text
/database/backup
/database/restore
/support/diagnostics
/support/diagnostics/download
```

When `OPENAPU_ADMIN_KEY` is empty, protection is disabled to preserve simple local use.

## Docker

Create a local `.env` file:

```text
OPENAPU_ADMIN_KEY=replace-with-a-long-random-value
```

Do not commit `.env`.

## Operational scripts

The administrative scripts accept the key in either of two ways.

Environment variable:

```powershell
$env:OPENAPU_ADMIN_KEY = "replace-with-a-long-random-value"
.\scripts\backup-openapu.ps1
```

Explicit parameter:

```powershell
.\scripts\backup-openapu.ps1 `
    -AdminKey "replace-with-a-long-random-value"
```

The same parameter is supported by:

```text
backup-openapu.ps1
backup-with-retention.ps1
collect-diagnostics.ps1
test-restore.ps1
```

Example direct request:

```powershell
Invoke-WebRequest `
    -Uri "http://localhost:8080/database/backup" `
    -Headers @{
        "X-OpenAPU-Admin-Key" = "replace-with-a-long-random-value"
    } `
    -OutFile ".\backups\openapu.db"
```

This shared key is a basic administrative control. It is not a replacement for HTTPS, user accounts or a reverse proxy when OpenAPU is exposed outside a trusted network.

## Key requirements

When protection is enabled, `OPENAPU_ADMIN_KEY` must contain at least 24 characters.

OpenAPU refuses to start when a shorter non-empty key is configured.

Use a long random value and keep it outside source control.
