using Riok.Mapperly.Abstractions;

using Wpf.Application.DTOs;
using Wpf.Domain.Entities;

namespace Wpf.Application.Mapping;

[Mapper]
public partial class CustomerMapper
{
    [MapperIgnoreSource(nameof(Customer.Orders))]
    public partial CustomerDto ToDto(Customer entity);

    public partial List<CustomerDto> ToDtoList(IEnumerable<Customer> entities);
}
