# Smoke test

OpenAPU includes a non-destructive operational smoke test.

It verifies:

- product identity;
- expected version;
- `/health`;
- `/ready`;
- `/system/status`;
- SQLite readiness;
- web interface availability;
- administrative protection when `OPENAPU_ADMIN_KEY` is configured.

Run against the default local instance:

```powershell
.\scripts\smoke-test.ps1
```

Run against another port:

```powershell
.\scripts\smoke-test.ps1 -Port 8081
```

Validate a protected instance:

```powershell
$env:OPENAPU_ADMIN_KEY = "your-long-administrative-key"
.\scripts\smoke-test.ps1
```

The smoke test does not create, modify or delete domain data.
