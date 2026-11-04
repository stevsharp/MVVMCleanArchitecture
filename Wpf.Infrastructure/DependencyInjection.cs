using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Wpf.Application.Abstractions;
using Wpf.Infrastructure.Data;
using Wpf.Infrastructure.Repositories;

namespace Wpf.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }

    /// <summary>Applies migrations and seeds the database. Call once at startup.</summary>
    public static Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        => services.GetRequiredService<IDatabaseInitializer>().InitializeAsync(cancellationToken);
}
