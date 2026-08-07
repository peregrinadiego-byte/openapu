using Microsoft.EntityFrameworkCore;

namespace OpenAPU.Infrastructure.Persistence;

public sealed class ResourceRow
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public string UnitSymbol { get; set; } = "";
    public string UnitName { get; set; } = "";
    public decimal Price { get; set; }
    public string Status { get; set; } = "";
}

public sealed class ApuRow
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public string UnitSymbol { get; set; } = "";
    public string UnitName { get; set; } = "";
    public List<ApuComponentRow> Components { get; set; } = [];
}

public sealed class ApuComponentRow
{
    public Guid Id { get; set; }
    public Guid ApuId { get; set; }
    public Guid ResourceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public ApuRow Apu { get; set; } = null!;
}

public sealed class ConceptRow
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public string UnitSymbol { get; set; } = "";
    public string UnitName { get; set; } = "";
    public Guid ApuId { get; set; }
    public decimal IndirectCost { get; set; }
    public decimal Financing { get; set; }
    public decimal Profit { get; set; }
    public decimal AdditionalCharges { get; set; }
}


public sealed class BudgetRow
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public List<BudgetItemRow> Items { get; set; } = [];
}

public sealed class BudgetItemRow
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public Guid ConceptId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public BudgetRow Budget { get; set; } = null!;
}
public sealed class OpenApuDbContext : DbContext
{
    public DbSet<ResourceRow> Resources => Set<ResourceRow>();
    public DbSet<ApuRow> Apus => Set<ApuRow>();
    public DbSet<ApuComponentRow> ApuComponents => Set<ApuComponentRow>();
    public DbSet<ConceptRow> Concepts => Set<ConceptRow>();
    public DbSet<BudgetRow> Budgets => Set<BudgetRow>();
    public DbSet<BudgetItemRow> BudgetItems => Set<BudgetItemRow>();

    public OpenApuDbContext(
        DbContextOptions<OpenApuDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var resource = modelBuilder.Entity<ResourceRow>();

        resource.ToTable("resources");
        resource.HasKey(row => row.Id);
        resource.Property(row => row.Id).ValueGeneratedNever();
        resource.HasIndex(row => row.Key).IsUnique();

        resource.Property(row => row.Key).HasMaxLength(100).IsRequired();
        resource.Property(row => row.Name).HasMaxLength(300).IsRequired();
        resource.Property(row => row.Type).HasMaxLength(50).IsRequired();
        resource.Property(row => row.UnitCode).HasMaxLength(50).IsRequired();
        resource.Property(row => row.UnitSymbol).HasMaxLength(30).IsRequired();
        resource.Property(row => row.UnitName).HasMaxLength(150).IsRequired();
        resource.Property(row => row.Status).HasMaxLength(30).IsRequired();

        var apu = modelBuilder.Entity<ApuRow>();

        apu.ToTable("apus");
        apu.HasKey(row => row.Id);
        apu.Property(row => row.Id).ValueGeneratedNever();
        apu.HasIndex(row => row.Key).IsUnique();

        apu.Property(row => row.Key).HasMaxLength(100).IsRequired();
        apu.Property(row => row.Name).HasMaxLength(300).IsRequired();
        apu.Property(row => row.UnitCode).HasMaxLength(50).IsRequired();
        apu.Property(row => row.UnitSymbol).HasMaxLength(30).IsRequired();
        apu.Property(row => row.UnitName).HasMaxLength(150).IsRequired();

        var component = modelBuilder.Entity<ApuComponentRow>();

        component.ToTable("apu_components");
        component.HasKey(row => row.Id);
        component.Property(row => row.Id).ValueGeneratedNever();
        component.HasIndex(row => new { row.ApuId, row.ResourceId }).IsUnique();

        component
            .HasOne(row => row.Apu)
            .WithMany(row => row.Components)
            .HasForeignKey(row => row.ApuId)
            .OnDelete(DeleteBehavior.Cascade);

        var concept = modelBuilder.Entity<ConceptRow>();

        concept.ToTable("concepts");
        concept.HasKey(row => row.Id);
        concept.Property(row => row.Id).ValueGeneratedNever();
        concept.HasIndex(row => row.Key).IsUnique();

        concept.Property(row => row.Key).HasMaxLength(100).IsRequired();
        concept.Property(row => row.Name).HasMaxLength(300).IsRequired();
        concept.Property(row => row.UnitCode).HasMaxLength(50).IsRequired();
        concept.Property(row => row.UnitSymbol).HasMaxLength(30).IsRequired();
        concept.Property(row => row.UnitName).HasMaxLength(150).IsRequired();

        var budget = modelBuilder.Entity<BudgetRow>();

        budget.ToTable("budgets");
        budget.HasKey(row => row.Id);
        budget.Property(row => row.Id).ValueGeneratedNever();
        budget.HasIndex(row => row.Key).IsUnique();

        budget.Property(row => row.Key).HasMaxLength(100).IsRequired();
        budget.Property(row => row.Name).HasMaxLength(300).IsRequired();

        var budgetItem = modelBuilder.Entity<BudgetItemRow>();

        budgetItem.ToTable("budget_items");
        budgetItem.HasKey(row => row.Id);
        budgetItem.Property(row => row.Id).ValueGeneratedNever();
        budgetItem.HasIndex(row => new { row.BudgetId, row.ConceptId }).IsUnique();

        budgetItem
            .HasOne(row => row.Budget)
            .WithMany(row => row.Items)
            .HasForeignKey(row => row.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


