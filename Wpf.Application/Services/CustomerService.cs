using FluentValidation;

using Wpf.Application.Abstractions;
using Wpf.Application.DTOs;
using Wpf.Application.Exceptions;
using Wpf.Application.Interfaces;
using Wpf.Application.Mapping;
using Wpf.Domain.Entities;

namespace Wpf.Application.Services;

public sealed class CustomerService(
    ICustomerRepository repository,
    IValidator<CustomerDto> validator,
    CustomerMapper customerMapper,
    OrderMapper orderMapper) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllAsync(cancellationToken);
        return customerMapper.ToDtoList(customers);
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetOrdersByCustomerIdAsync(customerId, cancellationToken);
        return orderMapper.ToDtoList(orders);
    }

    public async Task<CustomerDto> AddCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(dto, cancellationToken);

        var customer = new Customer(dto.Name);
        await repository.AddAsync(customer, cancellationToken);

        return customerMapper.ToDto(customer);
    }

    public async Task UpdateCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(dto, cancellationToken);

        var customer = await repository.GetByIdAsync(dto.Id, cancellationToken)
                       ?? throw new NotFoundException(nameof(Customer), dto.Id);

        customer.Rename(dto.Name);
        await repository.UpdateAsync(customer, cancellationToken);
    }

    public async Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
            throw new NotFoundException(nameof(Customer), id);
    }
}
