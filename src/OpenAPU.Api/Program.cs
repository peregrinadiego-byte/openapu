using OpenAPU.Api.Configuration;
using OpenAPU.Api.Observability;
using OpenAPU.Api.Security;
using OpenAPU.Api.Support;
using OpenAPU.Application.Abstractions;
using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Exports;
using OpenAPU.Application.Imports;
using OpenAPU.Application.Reports;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Backup;
using OpenAPU.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var adminAccessOptions = AdminAccessOptions.Create(
    builder.Configuration["OPENAPU_ADMIN_KEY"]
    ?? Environment.GetEnvironmentVariable(
        "OPENAPU_ADMIN_KEY"));

builder.Services.AddSingleton(adminAccessOptions);

var configuredDatabasePath =
    builder.Configuration["OPENAPU_DB_PATH"]
    ?? Environment.GetEnvironmentVariable("OPENAPU_DB_PATH");

var connectionString =
    builder.Configuration.GetConnectionString("OpenAPU")
    ?? "Data Source=openapu.db;Pooling=False";

var databasePath =
    !string.IsNullOrWhiteSpace(configuredDatabasePath)
        ? configuredDatabasePath
        : GetDatabasePath(connectionString);

var databaseStartupStatus =
    DatabaseStartupValidator.Validate(databasePath);

builder.Services.AddSingleton(databaseStartupStatus);
builder.Services.AddSingleton<DiagnosticsService>();

builder.Services.AddSingleton<IResourceRepository>(
    _ => new SqliteResourceRepository(databasePath));

builder.Services.AddSingleton<IApuRepository>(
    _ => new SqliteApuRepository(databasePath));

builder.Services.AddSingleton<IConceptRepository>(
    _ => new SqliteConceptRepository(databasePath));

builder.Services.AddSingleton(
    _ => new SqliteDatabaseTransferService(databasePath));
builder.Services.AddSingleton<IBudgetRepository>(
    _ => new SqliteBudgetRepository(databasePath));

builder.Services.AddSingleton<CreateResourceHandler>();
builder.Services.AddSingleton<CsvResourceImportService>();
builder.Services.AddSingleton<GetResourcesHandler>();
builder.Services.AddSingleton<UpdateResourceHandler>();

builder.Services.AddSingleton<CreateApuHandler>();
builder.Services.AddSingleton<AddApuComponentHandler>();
builder.Services.AddSingleton<GetApuHandler>();
builder.Services.AddSingleton<ChangeApuComponentQuantityHandler>();
builder.Services.AddSingleton<RemoveApuComponentHandler>();
builder.Services.AddSingleton<RefreshApuPricesHandler>();

builder.Services.AddSingleton<CreateConceptHandler>();
builder.Services.AddSingleton<UpdateConceptPercentagesHandler>();

builder.Services.AddSingleton<CreateBudgetHandler>();
builder.Services.AddSingleton<AddBudgetItemHandler>();
builder.Services.AddSingleton<GetResourceHandler>();
builder.Services.AddSingleton<GetApusHandler>();
builder.Services.AddSingleton<GetConceptHandler>();
builder.Services.AddSingleton<GetConceptsHandler>();
builder.Services.AddSingleton<GetBudgetHandler>();
builder.Services.AddSingleton<GetBudgetsHandler>();
builder.Services.AddSingleton<ChangeBudgetItemQuantityHandler>();
builder.Services.AddSingleton<RemoveBudgetItemHandler>();
builder.Services.AddSingleton<RefreshBudgetPricesHandler>();

var app = builder.Build();
_ = app.Services.GetRequiredService<IResourceRepository>();

app.UseMiddleware<RequestObservabilityMiddleware>();
app.UseMiddleware<AdminAccessMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        var exception = context.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
            ?.Error;

        context.Response.ContentType = "application/problem+json";

        context.Response.StatusCode = exception switch
        {
            OpenAPU.Domain.DomainException => StatusCodes.Status400BadRequest,
            OpenAPU.Application.ApplicationException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            status = context.Response.StatusCode,
            title = exception?.Message ?? "Unexpected error."
        });
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/app", () => Results.Redirect("/index.html"));
app.MapGet("/", () => Results.Ok(new
{
    name = "OpenAPU",
    version = "1.4.0",
    status = "ready"
}));

app.MapGet("/resources", async (
    GetResourcesHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(cancellationToken));
});

app.MapPost("/resources", async (
    CreateResourceCommand command,
    CreateResourceHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);

    return Results.Created(
        $"/resources/{result.Id}",
        result);
});

app.MapPut("/resources/{id:guid}", async (
    Guid id,
    UpdateResourceRequest request,
    UpdateResourceHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(
        new UpdateResourceCommand(
            id,
            request.Name,
            request.Price,
            request.IsActive),
        cancellationToken);

    return Results.Ok(result);
});

app.MapPost("/apus", async (
    CreateApuCommand command,
    CreateApuHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);

    return Results.Created(
        $"/apus/{result.Id}",
        result);
});

app.MapGet("/apus/{id:guid}", async (
    Guid id,
    GetApuHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(id, cancellationToken));
});

app.MapPost("/apus/{apuId:guid}/components", async (
    Guid apuId,
    AddApuComponentRequest request,
    AddApuComponentHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new AddApuComponentCommand(
                apuId,
                request.ResourceId,
                request.Quantity),
            cancellationToken));
});

app.MapPut("/apus/{apuId:guid}/components/{componentId:guid}", async (
    Guid apuId,
    Guid componentId,
    ChangeQuantityRequest request,
    ChangeApuComponentQuantityHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new ChangeApuComponentQuantityCommand(
                apuId,
                componentId,
                request.Quantity),
            cancellationToken));
});

app.MapDelete("/apus/{apuId:guid}/components/{componentId:guid}", async (
    Guid apuId,
    Guid componentId,
    RemoveApuComponentHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new RemoveApuComponentCommand(
                apuId,
                componentId),
            cancellationToken));
});

app.MapPost("/apus/{id:guid}/refresh-prices", async (
    Guid id,
    RefreshApuPricesHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(id, cancellationToken));
});

app.MapPost("/concepts", async (
    CreateConceptCommand command,
    CreateConceptHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);

    return Results.Created(
        $"/concepts/{result.Id}",
        result);
});

app.MapPut("/concepts/{id:guid}/percentages", async (
    Guid id,
    UpdateConceptPercentagesRequest request,
    UpdateConceptPercentagesHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new UpdateConceptPercentagesCommand(
                id,
                request.IndirectCost,
                request.Financing,
                request.Profit,
                request.AdditionalCharges),
            cancellationToken));
});

app.MapPost("/budgets", async (
    CreateBudgetCommand command,
    CreateBudgetHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(command, cancellationToken);

    return Results.Created(
        $"/budgets/{result.Id}",
        result);
});

app.MapPost("/budgets/{budgetId:guid}/items", async (
    Guid budgetId,
    AddBudgetItemRequest request,
    AddBudgetItemHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new AddBudgetItemCommand(
                budgetId,
                request.ConceptId,
                request.Quantity),
            cancellationToken));
});


app.MapGet("/resources/{id:guid}", async (
    Guid id,
    GetResourceHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(id, cancellationToken));
});

app.MapGet("/apus", async (
    GetApusHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(cancellationToken));
});

app.MapGet("/concepts", async (
    GetConceptsHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(cancellationToken));
});

app.MapGet("/concepts/{id:guid}", async (
    Guid id,
    GetConceptHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(id, cancellationToken));
});

app.MapGet("/budgets", async (
    GetBudgetsHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(cancellationToken));
});

app.MapGet("/budgets/{id:guid}", async (
    Guid id,
    GetBudgetHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(id, cancellationToken));
});

app.MapPut("/budgets/{budgetId:guid}/items/{itemId:guid}", async (
    Guid budgetId,
    Guid itemId,
    ChangeQuantityRequest request,
    ChangeBudgetItemQuantityHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new ChangeBudgetItemQuantityCommand(
                budgetId,
                itemId,
                request.Quantity),
            cancellationToken));
});

app.MapDelete("/budgets/{budgetId:guid}/items/{itemId:guid}", async (
    Guid budgetId,
    Guid itemId,
    RemoveBudgetItemHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(
        await handler.HandleAsync(
            new RemoveBudgetItemCommand(
                budgetId,
                itemId),
            cancellationToken));
});

app.MapPost("/budgets/{id:guid}/refresh-prices", async (
    Guid id,
    RefreshBudgetPricesHandler handler,
    CancellationToken cancellationToken) =>
{
    return Results.Ok(await handler.HandleAsync(id, cancellationToken));
});

app.MapGet("/exports/apus.csv", async (
    GetApusHandler handler,
    CancellationToken cancellationToken) =>
{
    var apus = await handler.HandleAsync(cancellationToken);
    var content = CsvExportService.ExportApus(apus);

    return Results.File(
        content,
        "text/csv; charset=utf-8",
        "openapu-apus.csv");
});

app.MapGet("/exports/budgets.csv", async (
    GetBudgetsHandler handler,
    CancellationToken cancellationToken) =>
{
    var budgets = await handler.HandleAsync(cancellationToken);
    var content = CsvExportService.ExportBudgets(budgets);

    return Results.File(
        content,
        "text/csv; charset=utf-8",
        "openapu-presupuestos.csv");
});

app.MapGet("/imports/resources/template.csv", () =>
{
    return Results.File(
        CsvResourceImportService.CreateTemplate(),
        "text/csv; charset=utf-8",
        "plantilla-recursos-openapu.csv");
});

app.MapPost("/imports/resources.csv", async (
    IFormFile file,
    CsvResourceImportService service,
    CancellationToken cancellationToken) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest(new
        {
            title = "El archivo estÃ¡ vacÃ­o."
        });
    }

    await using var stream = file.OpenReadStream();

    var result = await service.ImportAsync(
        stream,
        cancellationToken);

    return Results.Ok(result);
})
.DisableAntiforgery();

app.MapGet("/database/backup", async (
    SqliteDatabaseTransferService service,
    CancellationToken cancellationToken) =>
{
    var content = await service.CreateBackupAsync(
        cancellationToken);

    var timestamp = DateTimeOffset.UtcNow
        .ToString("yyyyMMdd-HHmmss");

    return Results.File(
        content,
        "application/vnd.sqlite3",
        $"openapu-backup-{timestamp}.db");
});

app.MapPost("/database/restore", async (
    IFormFile file,
    SqliteDatabaseTransferService service,
    CancellationToken cancellationToken) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest(new
        {
            title = "El archivo de respaldo estÃ¡ vacÃ­o."
        });
    }

    await using var stream = file.OpenReadStream();

    try
    {
        var result = await service.RestoreAsync(
            stream,
            cancellationToken);

        return Results.Ok(result);
    }
    catch (InvalidDataException exception)
    {
        return Results.BadRequest(new
        {
            title = exception.Message
        });
    }
})
.DisableAntiforgery();

app.MapGet("/reports/apus", async (
    GetApusHandler handler,
    CancellationToken cancellationToken) =>
{
    var apus = await handler.HandleAsync(cancellationToken);
    var html = HtmlReportService.CreateApuSummary(apus);

    return Results.Content(
        html,
        "text/html; charset=utf-8");
});

app.MapGet("/reports/budgets/{id:guid}", async (
    Guid id,
    GetBudgetHandler handler,
    CancellationToken cancellationToken) =>
{
    var budget = await handler.HandleAsync(
        id,
        cancellationToken);

    var html = HtmlReportService.CreateBudgetDetail(
        budget);

    return Results.Content(
        html,
        "text/html; charset=utf-8");
});

app.MapGet("/system/status", async (
    IResourceRepository resourceRepository,
    IApuRepository apuRepository,
    IConceptRepository conceptRepository,
    IBudgetRepository budgetRepository,
    CancellationToken cancellationToken) =>
{
    var resources = await resourceRepository.GetAllAsync(cancellationToken);
    var apus = await apuRepository.GetAllAsync(cancellationToken);
    var concepts = await conceptRepository.GetAllAsync(cancellationToken);
    var budgets = await budgetRepository.GetAllAsync(cancellationToken);

    return Results.Ok(new
    {
        name = "OpenAPU",
        version = "1.4.0",
        database = "ready",
        resources = resources.Count,
        apus = apus.Count,
        concepts = concepts.Count,
        budgets = budgets.Count,
        checkedAtUtc = DateTimeOffset.UtcNow
    });
});

app.MapGet("/ready", (
    DatabaseStartupStatus databaseStatus) =>
{
    return Results.Ok(new
    {
        name = "OpenAPU",
        version = "1.4.0",
        ready = databaseStatus.DirectoryExists &&
            databaseStatus.DirectoryWritable,
        databasePath = databaseStatus.Path,
        databaseDirectory = databaseStatus.Directory
    });
});

app.MapGet("/support/diagnostics", async (
    DiagnosticsService diagnosticsService,
    CancellationToken cancellationToken) =>
{
    var diagnostics = await diagnosticsService
        .CreateAsync(cancellationToken);

    return Results.Json(diagnostics);
});

app.MapGet("/support/diagnostics/download", async (
    DiagnosticsService diagnosticsService,
    CancellationToken cancellationToken) =>
{
    var diagnostics = await diagnosticsService
        .CreateAsync(cancellationToken);

    var json = System.Text.Json.JsonSerializer.Serialize(
        diagnostics,
        new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

    var bytes = System.Text.Encoding.UTF8.GetBytes(json);
    var fileName =
        $"openapu-diagnostics-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.json";

    return Results.File(
        bytes,
        "application/json",
        fileName);
});
app.Run();

static string GetDatabasePath(string connectionString)
{
    const string prefix = "Data Source=";

    var segment = connectionString
        .Split(';', StringSplitOptions.RemoveEmptyEntries)
        .FirstOrDefault(part =>
            part.Trim().StartsWith(
                prefix,
                StringComparison.OrdinalIgnoreCase));

    if (segment is null)
    {
        return "openapu.db";
    }

    return segment.Trim()[prefix.Length..].Trim();
}

public sealed record UpdateResourceRequest(
    string Name,
    decimal Price,
    bool IsActive);

public sealed record AddApuComponentRequest(
    Guid ResourceId,
    decimal Quantity);

public sealed record ChangeQuantityRequest(
    decimal Quantity);

public sealed record UpdateConceptPercentagesRequest(
    decimal IndirectCost,
    decimal Financing,
    decimal Profit,
    decimal AdditionalCharges);

public sealed record AddBudgetItemRequest(
    Guid ConceptId,
    decimal Quantity);

public partial class Program;



















