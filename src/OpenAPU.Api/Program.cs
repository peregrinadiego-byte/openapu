using OpenAPU.Application.Abstractions;
using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("OpenAPU")
    ?? "Data Source=openapu.db;Pooling=False";

var databasePath = GetDatabasePath(connectionString);

builder.Services.AddSingleton<IResourceRepository>(
    _ => new SqliteResourceRepository(databasePath));

builder.Services.AddSingleton<IApuRepository>(
    _ => new SqliteApuRepository(databasePath));

builder.Services.AddSingleton<IConceptRepository>(
    _ => new SqliteConceptRepository(databasePath));

builder.Services.AddSingleton<IBudgetRepository>(
    _ => new SqliteBudgetRepository(databasePath));

builder.Services.AddSingleton<CreateResourceHandler>();
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

var app = builder.Build();

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
