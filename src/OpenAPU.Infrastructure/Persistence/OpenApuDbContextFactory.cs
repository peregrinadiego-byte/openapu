using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenAPU.Infrastructure.Persistence;

public sealed class OpenApuDbContextFactory :
    IDesignTimeDbContextFactory<OpenApuDbContext>
{
    public OpenApuDbContext CreateDbContext(string[] args)
    {
        var databasePath =
            Environment.GetEnvironmentVariable("OPENAPU_DB_PATH")
            ?? Path.Combine(
                Directory.GetCurrentDirectory(),
                "openapu.db");

        var options = new DbContextOptionsBuilder<OpenApuDbContext>()
            .UseSqlite($"Data Source={databasePath};Pooling=False")
            .Options;

        return new OpenApuDbContext(options);
    }
}
