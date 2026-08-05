# Backup integrity

OpenAPU backups are SQLite database files.

The backup process now verifies:

- the file was created;
- the file is not empty;
- the SQLite header is valid;
- a SHA-256 checksum is generated.

Create a backup:

```powershell
.\scripts\backup-openapu.ps1
```

Verify an existing backup:

```powershell
.\scripts\verify-backup.ps1 `
    -BackupPath .\backups\openapu-YYYYMMDD-HHMMSS.db
```

Create a backup and retain only the ten newest copies:

```powershell
.\scripts\backup-with-retention.ps1
```

A valid checksum confirms file integrity, not that the backup contains the intended business data. Periodic restoration tests remain necessary.
