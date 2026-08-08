# OpenAPU 1.4.0

## Alcance

OpenAPU 1.4.0 consolida seguridad administrativa y recuperación operativa.

## Cambios principales

- clave administrativa opcional mediante `OPENAPU_ADMIN_KEY`;
- longitud mínima obligatoria de 24 caracteres cuando la protección está activa;
- comparación de clave en tiempo constante;
- protección de respaldo, restauración y diagnóstico;
- integración de la clave con scripts operativos;
- diagnóstico JSON descargable;
- checksum SHA-256 para respaldos;
- validación de cabecera SQLite;
- retención de respaldos;
- prueba real de restauración en volumen Docker temporal;
- validación de integridad SQLite mediante `PRAGMA integrity_check`;
- 100 pruebas automatizadas.

## Compatibilidad

La estructura de datos y el núcleo funcional se mantienen compatibles con OpenAPU 1.3.0.

## Verificación

```powershell
dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln
dotnet test OpenAPU.sln
```

## Respaldo

```powershell
.\scripts\backup-openapu.ps1
.\scripts\verify-backup.ps1 -BackupPath <archivo.db>
```

## Prueba de restauración

```powershell
.\scripts\test-restore.ps1
```

## Diagnóstico

```powershell
.\scripts\collect-diagnostics.ps1
```
