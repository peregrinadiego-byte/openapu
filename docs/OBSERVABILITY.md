# Operational observability

OpenAPU assigns a correlation identifier to every HTTP request.

Header:

```text
X-Correlation-ID
```

Clients may provide their own value. When omitted, OpenAPU generates one.

Each completed request is logged with:

- HTTP method;
- path;
- status code;
- elapsed time;
- correlation identifier.

Unhandled exceptions are logged with the same identifier, allowing a user-visible failure to be matched with the corresponding server log entry.

This is a local operational baseline. Centralized log storage remains outside the current project scope.
