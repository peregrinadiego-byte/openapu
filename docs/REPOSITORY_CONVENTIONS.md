# Repository conventions

OpenAPU keeps generated files outside version control.

Ignored local outputs include:

- build folders;
- test results;
- release archives;
- backups;
- SQLite databases;
- local data volumes.

Line endings are normalized through `.gitattributes`.

Formatting and whitespace rules are defined in `.editorconfig`.

Before committing repository-maintenance changes, run:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\check-repository.ps1
```

This verifies:

1. generated files are not tracked;
2. Git attributes are active;
3. .NET formatting is consistent;
4. the solution builds;
5. the complete test suite passes.
