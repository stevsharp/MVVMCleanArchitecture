using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Wpf.Domain.Entities;
using Wpf.Infrastructure.Data;
using Wpf.Infrastructure.Repositories;

namespace Wpf.Tests.Infrastructure;

/// <summary>
/// Exercises the EF repository against a real SQLite in-memory database.
/// </summary>
public sealed class CustomerRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        // An in-memory SQLite database lives as long as one connection stays open.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var db = new AppDbContext(options))
            db.Database.EnsureCreated();

        _repository = new CustomerRepository(new SharedConnectionContextFactory(options));
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task AddAsync_assigns_id_and_persists_orders()
    {
        var alice = new Customer("Alice");
        alice.AddOrder("A1");

        await _repository.AddAsync(alice);

        Assert.NotEqual(0, alice.Id);
        var orders = await _repository.GetOrdersByCustomerIdAsync(alice.Id);
        var order = Assert.Single(orders);
        Assert.Equal("A1", order.Description);
        Assert.Equal(alice.Id, order.CustomerId);
    }

    [Fact]
    public async Task GetAllAsync_returns_customers_ordered_by_id()
    {
        await _repository.AddAsync(new Customer("Bob"));
        await _repository.AddAsync(new Customer("Alice"));

        var all = await _repository.GetAllAsync();

        Assert.Equal(["Bob", "Alice"], all.Select(c => c.Name));
    }

    [Fact]
    public async Task UpdateAsync_persists_changes_made_on_a_detached_entity()
    {
        var alice = new Customer("Alice");
        await _repository.AddAsync(alice);

        var detached = await _repository.GetByIdAsync(alice.Id);
        detached!.Rename("Alicia");
        await _repository.UpdateAsync(detached);

        var reloaded = await _repository.GetByIdAsync(alice.Id);
        Assert.Equal("Alicia", reloaded!.Name);
    }

    [Fact]
    public async Task DeleteAsync_removes_customer_and_cascades_orders()
    {
        var alice = new Customer("Alice");
        alice.AddOrder("A1");
        await _repository.AddAsync(alice);

        var deleted = await _repository.DeleteAsync(alice.Id);

        Assert.True(deleted);
        Assert.Null(await _repository.GetByIdAsync(alice.Id));
        Assert.Empty(await _repository.GetOrdersByCustomerIdAsync(alice.Id));
    }

    [Fact]
    public async Task DeleteAsync_returns_false_for_unknown_id()
    {
        Assert.False(await _repository.DeleteAsync(999));
    }

    private sealed class SharedConnectionContextFactory(DbContextOptions<AppDbContext> options)
        : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
    }
}
