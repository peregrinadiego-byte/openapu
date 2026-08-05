# Release process

OpenAPU publishes downloadable packages when a Git tag matching `v*` is pushed.

## Automated process

The release workflow:

1. restores dependencies;
2. builds in Release mode;
3. runs the complete test suite;
4. publishes `win-x64` and `linux-x64`;
5. creates ZIP packages;
6. creates a GitHub Release;
7. attaches both packages to the release.

Workflow:

```text
.github/workflows/release.yml
```

## Local package verification

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\package-release.ps1
```

Generated packages are placed in:

```text
artifacts\
```

## Publishing a new release

After updating the version and release notes:

```powershell
git add .
git commit -m "Release OpenAPU X.Y.Z"
git tag -a vX.Y.Z -m "OpenAPU X.Y.Z"
git push origin master
git push origin vX.Y.Z
```

The tag push starts the release workflow.
