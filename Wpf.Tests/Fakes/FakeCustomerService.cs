using Wpf.Application.DTOs;
using Wpf.Application.Exceptions;
using Wpf.Application.Interfaces;

namespace Wpf.Tests.Fakes;

/// <summary>
/// Scriptable <see cref="ICustomerService"/> for view-model tests.
/// Set <see cref="ThrowOnNextCall"/> to make the next call fail.
/// </summary>
public sealed class FakeCustomerService : ICustomerService
{
    private int _nextId = 1;

    public List<CustomerDto> Customers { get; } = [];
    public Dictionary<int, List<OrderDto>> OrdersByCustomer { get; } = [];
    public Exception? ThrowOnNextCall { get; set; }

    public CustomerDto Seed(string name, params string[] orders)
    {
        var customer = new CustomerDto(_nextId++, name);
        Customers.Add(customer);
        OrdersByCustomer[customer.Id] = orders
            .Select((d, i) => new OrderDto(i + 1, d, customer.Id))
            .ToList();
        return customer;
    }

    public Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfScripted();
        return Task.FromResult<IReadOnlyList<CustomerDto>>(Customers.ToList());
    }

    public Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        ThrowIfScripted();
        var orders = OrdersByCustomer.GetValueOrDefault(customerId) ?? [];
        return Task.FromResult<IReadOnlyList<OrderDto>>(orders);
    }

    public Task<CustomerDto> AddCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default)
    {
        ThrowIfScripted();
        var created = dto with { Id = _nextId++ };
        Customers.Add(created);
        return Task.FromResult(created);
    }

    public Task UpdateCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default)
    {
        ThrowIfScripted();
        var index = Customers.FindIndex(c => c.Id == dto.Id);
        if (index < 0) throw new NotFoundException("Customer", dto.Id);
        Customers[index] = dto;
        return Task.CompletedTask;
    }

    public Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        ThrowIfScripted();
        if (Customers.RemoveAll(c => c.Id == id) == 0) throw new NotFoundException("Customer", id);
        return Task.CompletedTask;
    }

    private void ThrowIfScripted()
    {
        if (ThrowOnNextCall is null) return;
        var ex = ThrowOnNextCall;
        ThrowOnNextCall = null;
        throw ex;
    }
}
