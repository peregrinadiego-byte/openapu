# Deployment security baseline

OpenAPU applies a conservative deployment baseline:

- the .NET container runs as the non-root `app` user;
- Linux capabilities are dropped;
- privilege escalation is disabled;
- the container filesystem is read-only;
- `/tmp` remains available as temporary memory-backed storage;
- persistent writes are limited to `/data`;
- defensive HTTP headers are added to all responses.

This baseline does not replace TLS, authentication, network controls or operating-system updates.

For deployment outside a trusted local network, place OpenAPU behind a reverse proxy with HTTPS and explicit access control.
