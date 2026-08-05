# OpenAPU 1.1.0

## Estado

Versión funcional posterior al núcleo 1.0.

## Incluye

- recursos;
- análisis de precios unitarios;
- conceptos;
- presupuestos;
- persistencia SQLite;
- migraciones EF Core;
- API HTTP;
- interfaz web;
- edición y eliminación;
- importación y exportación CSV;
- respaldo y restauración;
- reportes imprimibles;
- validación y navegación mejoradas;
- diagnóstico general del sistema.

## Compatibilidad

La estructura de datos se mantiene compatible con OpenAPU 1.0.

## Verificación

```powershell
dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln
dotnet test OpenAPU.sln
```

Para ejecutar:

```powershell
dotnet run --project .\src\OpenAPU.Api\OpenAPU.Api.csproj
```

Comprobaciones:

```text
GET /
GET /health
GET /system/status
```
