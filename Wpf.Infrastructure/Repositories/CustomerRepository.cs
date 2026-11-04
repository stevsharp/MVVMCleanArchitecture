using Microsoft.EntityFrameworkCore;

using Wpf.Application.Abstractions;
using Wpf.Domain.Entities;
using Wpf.Infrastructure.Data;

namespace Wpf.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ICustomerRepository"/>.
/// A fresh <see cref="AppDbContext"/> is created for every call, so entities are always returned detached.
/// </summary>
public sealed class CustomerRepository(IDbContextFactory<AppDbContext> dbContextFactory) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderBy(o => o.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        db.Customers.Add(customer);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        // The entity was loaded by a different context, so attach it and mark it modified.
        db.Customers.Update(customer);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var deleted = await db.Customers
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}
