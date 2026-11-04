using Wpf.Application.DTOs;

namespace Wpf.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>Validates and persists a new customer. Returns the created customer with its generated id.</summary>
    /// <exception cref="FluentValidation.ValidationException">The DTO failed validation.</exception>
    Task<CustomerDto> AddCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default);

    /// <exception cref="FluentValidation.ValidationException">The DTO failed validation.</exception>
    /// <exception cref="Exceptions.NotFoundException">No customer with the given id exists.</exception>
    Task UpdateCustomerAsync(CustomerDto dto, CancellationToken cancellationToken = default);

    /// <exception cref="Exceptions.NotFoundException">No customer with the given id exists.</exception>
    Task DeleteCustomerAsync(int id, CancellationToken cancellationToken = default);
}
