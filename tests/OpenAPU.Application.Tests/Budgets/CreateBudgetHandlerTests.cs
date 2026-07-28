using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Tests.Apus;
using OpenAPU.Application.Tests.Concepts;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Budgets;

public sealed class CreateBudgetHandlerTests
{
    [Fact]
    public async Task Creates_empty_budget()
    {
        var repository = new FakeBudgetRepository();
        var handler = new CreateBudgetHandler(repository);

        var result = await handler.HandleAsync(
            new CreateBudgetCommand(
                "PRE-001",
                "Presupuesto de obra"));

        Assert.Equal("PRE-001", result.Key);
        Assert.Equal("Presupuesto de obra", result.Name);
        Assert.Equal(0m, result.Total);
        Assert.Empty(result.Items);
        Assert.Single(repository.Budgets);
    }

    [Fact]
    public async Task Rejects_duplicate_key()
    {
        var repository = new FakeBudgetRepository();
        var handler = new CreateBudgetHandler(repository);

        var command = new CreateBudgetCommand(
            "PRE-001",
            "Presupuesto");

        await handler.HandleAsync(command);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(command));
    }
}
