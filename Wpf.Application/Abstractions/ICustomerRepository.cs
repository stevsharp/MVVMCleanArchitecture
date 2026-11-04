using Wpf.Domain.Entities;

namespace Wpf.Application.Abstractions;

/// <summary>
/// Persistence boundary for the <see cref="Customer"/> aggregate.
/// Each call is its own unit of work; entities returned are detached.
/// </summary>
public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
