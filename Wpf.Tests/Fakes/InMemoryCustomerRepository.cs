using Wpf.Application.Abstractions;
using Wpf.Domain.Entities;

namespace Wpf.Tests.Fakes;

/// <summary>
/// Simple in-memory <see cref="ICustomerRepository"/>. Assigns ids on add, like a database would.
/// </summary>
public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<int, Customer> _customers = [];
    private int _nextCustomerId = 1;
    private int _nextOrderId = 1;

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Customer>>(_customers.Values.OrderBy(c => c.Id).ToList());

    public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_customers.GetValueOrDefault(id));

    public Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var orders = _customers.TryGetValue(customerId, out var customer)
            ? customer.Orders.OrderBy(o => o.Id).ToList()
            : [];

        return Task.FromResult<IReadOnlyList<Order>>(orders);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        customer.Id = _nextCustomerId++;
        foreach (var order in customer.Orders)
        {
            order.Id = _nextOrderId++;
            order.CustomerId = customer.Id;
        }

        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(_customers.Remove(id));
}
