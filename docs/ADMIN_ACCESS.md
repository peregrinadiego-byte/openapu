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

When `OPENAPU_ADMIN_KEY` is empty, the protection is disabled to preserve simple local use.

For a protected Docker deployment, create a `.env` file:

```text
OPENAPU_ADMIN_KEY=replace-with-a-long-random-value
```

Example request:

```powershell
Invoke-WebRequest `
    -Uri "http://localhost:8080/database/backup" `
    -Headers @{
        "X-OpenAPU-Admin-Key" = "replace-with-a-long-random-value"
    } `
    -OutFile ".\backups\openapu.db"
```

This shared key is a basic administrative control. It is not a replacement for HTTPS, user accounts or a reverse proxy when OpenAPU is exposed outside a trusted network.
