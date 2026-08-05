# OpenAPU 1.2.0

## Alcance

OpenAPU 1.2.0 consolida la distribuciÃ³n y operaciÃ³n reproducible del sistema.

## Cambios principales

- despliegue con Docker Compose;
- almacenamiento persistente mediante volumen;
- verificaciÃ³n automÃ¡tica de salud y estado;
- scripts de inicio, detenciÃ³n, respaldo y diagnÃ³stico;
- integraciÃ³n continua para compilaciÃ³n y pruebas;
- validaciÃ³n automÃ¡tica de la imagen Docker;
- generaciÃ³n de paquetes para Windows y Linux;
- publicaciÃ³n automÃ¡tica de GitHub Releases;
- documentaciÃ³n del proceso de entrega.

## Compatibilidad

La estructura funcional y de datos se mantiene compatible con OpenAPU 1.1.0.

## VerificaciÃ³n

```powershell
dotnet restore OpenAPU.sln
dotnet build OpenAPU.sln
dotnet test OpenAPU.sln
```

Empaquetado local:

```powershell
.\scripts\package-release.ps1
```

Despliegue:

```powershell
docker compose up -d --build
.\scripts\verify-deployment.ps1
```
