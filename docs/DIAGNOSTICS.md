# Diagnostics package

OpenAPU can generate a support-safe diagnostics file.

Endpoints:

```text
GET /support/diagnostics
GET /support/diagnostics/download
```

The diagnostics include:

- product and version;
- generation time;
- runtime and operating system;
- effective SQLite path;
- database readiness;
- counts of resources, APU, concepts and budgets.

The diagnostics do not include:

- resource names;
- APU contents;
- budget contents;
- user credentials;
- backup files.

Download from PowerShell:

```powershell
.\scripts\collect-diagnostics.ps1
```

Files are saved in the local `diagnostics` directory.
