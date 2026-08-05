# Restore drill

A backup is useful only when it can be restored.

OpenAPU includes a recovery drill that:

1. creates a new backup;
2. validates its SHA-256 checksum;
3. copies it into a temporary Docker volume;
4. starts an isolated OpenAPU container;
5. verifies `/ready`;
6. verifies database status;
7. removes the temporary container and volume.

Run while the main OpenAPU instance is active:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\test-restore.ps1
```

Defaults:

```text
Source instance: http://localhost:8080
Temporary test:  http://localhost:8082
Image:           openapu:1.3.0
```

This test does not replace an external backup policy. Important copies should also be stored outside the host running OpenAPU.
