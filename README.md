# OpenAPU Kernel 1.0

Implementación de referencia inicial del dominio OpenAPU en C# y .NET 8.

## Contenido

- Value Objects: `Identifier`, `Key`, `Money`, `Quantity`, `Percentage`, `Unit`.
- Entidades y agregados: `Resource`, `Apu`, `Concept`, `Budget`.
- Entidades internas: `ApuComponent`, `BudgetItem`.
- Prueba integral de recursos → APU → concepto → presupuesto.

## Ejecutar

```bash
dotnet restore
dotnet test OpenAPU.sln
```

## Cobertura

```bash
dotnet test OpenAPU.sln --collect:"XPlat Code Coverage"
```

## Alcance

Este repositorio contiene únicamente el Kernel. No incluye base de datos, API, interfaz ni importación/exportación.
