using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wpf.Infrastructure.Data;

/// <summary>
/// Used only by the <c>dotnet ef</c> tooling so migrations can be added without a startup project.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=app.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
