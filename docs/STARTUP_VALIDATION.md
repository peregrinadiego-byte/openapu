# Startup validation

OpenAPU validates the SQLite path before repositories are created.

The validation confirms:

- the configured path can be resolved;
- the parent directory exists or can be created;
- the process can write to the directory;
- the application can expose the resolved path through the readiness endpoint.

Endpoint:

```text
GET /ready
```

A successful response includes:

```json
{
  "name": "OpenAPU",
  "version": "1.2.0",
  "ready": true
}
```

This endpoint is intended for local checks, Docker health probes and reverse proxies.
