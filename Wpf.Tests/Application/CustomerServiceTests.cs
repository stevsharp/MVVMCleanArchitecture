using FluentValidation;

using Wpf.Application.DTOs;
using Wpf.Application.Exceptions;
using Wpf.Application.Mapping;
using Wpf.Application.Services;
using Wpf.Application.Validators;
using Wpf.Domain.Entities;
using Wpf.Tests.Fakes;

namespace Wpf.Tests.Application;

public class CustomerServiceTests
{
    private readonly InMemoryCustomerRepository _repository = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(_repository, new CustomerValidator(), new CustomerMapper(), new OrderMapper());
    }

    [Fact]
    public async Task GetAllCustomersAsync_maps_entities_to_dtos()
    {
        await _repository.AddAsync(new Customer("Alice"));
        await _repository.AddAsync(new Customer("Bob"));

        var result = await _service.GetAllCustomersAsync();

        Assert.Equal(["Alice", "Bob"], result.Select(c => c.Name));
        Assert.All(result, c => Assert.NotEqual(0, c.Id));
    }

    [Fact]
    public async Task GetOrdersByCustomerIdAsync_returns_only_that_customers_orders()
    {
        var alice = new Customer("Alice");
        alice.AddOrder("A1");
        alice.AddOrder("A2");
        var bob = new Customer("Bob");
        bob.AddOrder("B1");
        await _repository.AddAsync(alice);
        await _repository.AddAsync(bob);

        var result = await _service.GetOrdersByCustomerIdAsync(alice.Id);

        Assert.Equal(["A1", "A2"], result.Select(o => o.Description));
        Assert.All(result, o => Assert.Equal(alice.Id, o.CustomerId));
    }

    [Fact]
    public async Task AddCustomerAsync_persists_and_returns_dto_with_id()
    {
        var created = await _service.AddCustomerAsync(new CustomerDto(0, "Alice"));

        Assert.NotEqual(0, created.Id);
        Assert.Equal("Alice", created.Name);
        Assert.NotNull(await _repository.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task AddCustomerAsync_throws_on_invalid_dto_without_persisting()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _service.AddCustomerAsync(new CustomerDto(0, "")));

        Assert.Empty(await _repository.GetAllAsync());
    }

    [Fact]
    public async Task UpdateCustomerAsync_renames_existing_customer()
    {
        var created = await _service.AddCustomerAsync(new CustomerDto(0, "Alice"));

        await _service.UpdateCustomerAsync(created with { Name = "Alicia" });

        var stored = await _repository.GetByIdAsync(created.Id);
        Assert.Equal("Alicia", stored!.Name);
    }

    [Fact]
    public async Task UpdateCustomerAsync_throws_NotFound_for_unknown_id()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateCustomerAsync(new CustomerDto(999, "Nobody")));
    }

    [Fact]
    public async Task DeleteCustomerAsync_removes_customer()
    {
        var created = await _service.AddCustomerAsync(new CustomerDto(0, "Alice"));

        await _service.DeleteCustomerAsync(created.Id);

        Assert.Null(await _repository.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteCustomerAsync_throws_NotFound_for_unknown_id()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteCustomerAsync(999));
    }
}
