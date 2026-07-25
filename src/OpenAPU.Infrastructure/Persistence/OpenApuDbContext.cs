using Microsoft.EntityFrameworkCore;

namespace OpenAPU.Infrastructure.Persistence;

internal sealed class ResourceRow
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

internal sealed class OpenApuDbContext : DbContext
{
    public DbSet<ResourceRow> Resources => Set<ResourceRow>();

    public OpenApuDbContext(DbContextOptions<OpenApuDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var resource = modelBuilder.Entity<ResourceRow>();

        resource.ToTable("resources");
        resource.HasKey(row => row.Id);
        resource.HasIndex(row => row.Key).IsUnique();

        resource.Property(row => row.Key).HasMaxLength(100).IsRequired();
        resource.Property(row => row.Name).HasMaxLength(300).IsRequired();
        resource.Property(row => row.Type).HasMaxLength(50).IsRequired();
        resource.Property(row => row.UnitCode).HasMaxLength(50).IsRequired();
        resource.Property(row => row.UnitSymbol).HasMaxLength(30).IsRequired();
        resource.Property(row => row.UnitName).HasMaxLength(150).IsRequired();
        resource.Property(row => row.Status).HasMaxLength(30).IsRequired();
    }
}
