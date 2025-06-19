using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Wpf.Application.DTOs;
using Wpf.Domain.Entities;
using Wpf.Infrastructure.Mapping;

public class CustomerService(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IValidator<CustomerDto> validator ,
    CustomerMapper customerMapper,
    OrderMapper orderMapper) : ICustomerService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;
    private readonly CustomerMapper _customerMapper = customerMapper;
    private readonly OrderMapper _orderMapper = orderMapper;
    private readonly IValidator<CustomerDto> _customerValidator = validator;

    public async ValueTask<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entities = await db.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _customerMapper.ToDtoList(entities);
    }

    public async ValueTask<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        return _orderMapper.ToDtoList(orders);
    }

    public async ValueTask<int> AddCustomerAsync(CustomerDto dto, CancellationToken token)
    {
        var result = _customerValidator.Validate(dto);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        using var db = await _dbContextFactory.CreateDbContextAsync(token);
        var customer = new Customer(dto.Name);
        db.Customers.Add(customer);
        await db.SaveChangesAsync(token);
        return customer.Id;
    }

    public async ValueTask UpdateCustomerAsync(CustomerDto dto, CancellationToken token)
    {
        var result = _customerValidator.Validate(dto);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        using var db = await _dbContextFactory.CreateDbContextAsync(token);
        var customer = await db.Customers.FindAsync([dto.Id], token);
        
        if (customer is null) 
            throw new Exception("Customer not found.");

        customer.Rename(dto.Name); // assuming domain method
        await db.SaveChangesAsync(token);
    }

    public async ValueTask DeleteCustomerAsync(int id, CancellationToken token)
    {
        using var db = await _dbContextFactory.CreateDbContextAsync(token);
        var entity = await db.Customers.FindAsync([id], token);
        if (entity is null) return;
        db.Customers.Remove(entity);
        await db.SaveChangesAsync(token);
    }
}
