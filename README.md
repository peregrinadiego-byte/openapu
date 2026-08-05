# OpenAPU 1.1.0

NÃºcleo abierto para administrar recursos, anÃ¡lisis de precios unitarios, conceptos y presupuestos de obra.

## Requisitos

- .NET SDK 8
- SQLite
- Docker, opcional

## Ejecutar localmente

```powershell
dotnet tool restore
dotnet restore OpenAPU.sln
dotnet run --project src/OpenAPU.Api/OpenAPU.Api.csproj
```

La API queda disponible en:

```text
http://localhost:5080
```

ComprobaciÃ³n:

```text
GET /health
```

## Ejecutar pruebas

```powershell
dotnet test OpenAPU.sln
```

## Base de datos

La aplicaciÃ³n usa SQLite y aplica automÃ¡ticamente las migraciones pendientes.

La ruta predeterminada es:

```text
openapu.db
```

Puede cambiarse mediante:

```powershell
$env:OPENAPU_DB_PATH = "C:\datos\openapu.db"
```

## Docker

```powershell
docker build -t openapu:1.0 .
docker run --rm -p 8080:8080 -v openapu-data:/data openapu:1.0
```

La API queda disponible en:

```text
http://localhost:8080
```

## Alcance 1.0

- recursos;
- anÃ¡lisis de precios unitarios;
- componentes de APU;
- conceptos y porcentajes;
- presupuestos y partidas;
- persistencia SQLite;
- API HTTP;
- migraciones y pruebas automatizadas.

OpenAPU 1.1.0 es una versiÃ³n funcional con interfaz, intercambio de datos, respaldo y reportes. No incluye todavÃ­a interfaz grÃ¡fica, autenticaciÃ³n, reportes ni importaciÃ³n desde otros sistemas.


## Despliegue con Docker Compose

Iniciar:

```powershell
.\scripts\start-openapu.ps1
```

Abrir:

```text
http://localhost:8080
```

Detener sin borrar los datos:

```powershell
.\scripts\stop-openapu.ps1
```

Crear un respaldo:

```powershell
.\scripts\backup-openapu.ps1
```

Verificar una instancia activa:

```powershell
.\scripts\verify-deployment.ps1
```

Los datos se conservan en el volumen `openapu-data`.

## IntegraciÃ³n continua

GitHub Actions compila, prueba y valida la imagen Docker en cada cambio enviado a las ramas `master` o `main`.

Consulta:

```text
docs/CONTINUOUS_INTEGRATION.md
```
