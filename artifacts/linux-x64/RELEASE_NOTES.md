# OpenAPU 1.3.0

## Alcance

OpenAPU 1.3.0 consolida la operaciÃ³n segura y verificable del sistema.

## Cambios principales

- contenedor ejecutado con usuario no privilegiado;
- sistema de archivos del contenedor en solo lectura;
- eliminaciÃ³n de capacidades Linux;
- bloqueo de escalamiento de privilegios;
- ruta SQLite configurable mediante `OPENAPU_DB_PATH`;
- correcciÃ³n de permisos persistentes para `/data`;
- encabezados HTTP defensivos;
- identificadores de correlaciÃ³n por solicitud;
- registros de mÃ©todo, ruta, estado y duraciÃ³n;
- trazabilidad de excepciones;
- validaciÃ³n de la ruta y permisos de SQLite al iniciar;
- endpoint de preparaciÃ³n `GET /ready`;
- mantenimiento automatizado de dependencias de GitHub Actions;
- convenciones de formato y finales de lÃ­nea.

## Compatibilidad

La estructura funcional y de datos se mantiene compatible con OpenAPU 1.2.0.

## VerificaciÃ³n

```powershell
dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln
dotnet test OpenAPU.sln
```

Despliegue:

```powershell
docker compose up -d --build
.\scripts\verify-deployment.ps1
```

PreparaciÃ³n:

```text
GET /ready
```
