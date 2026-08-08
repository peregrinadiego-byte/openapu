# Initial setup

OpenAPU can be prepared and started with one command.

From the repository root:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\initialize-openapu.ps1
```

The script:

1. verifies Docker;
2. creates `.env` only when it does not already exist;
3. generates a random 64-character administrative key;
4. builds and starts OpenAPU;
5. waits for `/ready`;
6. executes the operational smoke test.

The generated `.env` remains local and is excluded from Git.

To start without administrative protection:

```powershell
.\scripts\initialize-openapu.ps1 -WithoutAdminKey
```

Existing `.env` files are never overwritten by the initialization script.
