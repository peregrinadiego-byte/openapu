# GitHub Actions maintenance

OpenAPU uses maintained action versions compatible with the current GitHub-hosted runner runtime.

Current artifact actions:

```text
actions/upload-artifact@v6
actions/download-artifact@v8
softprops/action-gh-release@v3
```

Dependabot checks GitHub Actions dependencies monthly and proposes updates through pull requests.

Configuration:

```text
.github/dependabot.yml
```

Action upgrades must pass the complete continuous-integration workflow before merging.
