# Continuous integration

OpenAPU uses GitHub Actions to verify every push and pull request.

The workflow performs:

1. dependency restoration;
2. Release compilation;
3. complete automated test execution;
4. test-result publication;
5. Docker image construction;
6. container startup;
7. health, version and database-status checks.

Workflow file:

```text
.github/workflows/ci.yml
```

A successful workflow confirms that the source code and Docker image are reproducible in a clean Linux environment.
