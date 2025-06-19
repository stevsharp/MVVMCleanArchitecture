using Wpf.Application.DTOs;

public interface ICustomerService
{
    ValueTask<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken);
    ValueTask<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    ValueTask<int> AddCustomerAsync(CustomerDto dto, CancellationToken token);
    ValueTask UpdateCustomerAsync(CustomerDto dto, CancellationToken token);
    ValueTask DeleteCustomerAsync(int id, CancellationToken token);
}