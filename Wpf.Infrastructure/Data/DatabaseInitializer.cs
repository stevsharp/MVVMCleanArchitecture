using Microsoft.EntityFrameworkCore;

namespace Wpf.Infrastructure.Data;

public interface IDatabaseInitializer
{
    /// <summary>Applies pending migrations and seeds initial data if the database is empty.</summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public sealed class DatabaseInitializer(IDbContextFactory<AppDbContext> dbContextFactory) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await db.Database.MigrateAsync(cancellationToken);
        await AppDbSeeder.SeedAsync(db, cancellationToken);
    }
}
