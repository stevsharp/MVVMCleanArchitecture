using Riok.Mapperly.Abstractions;


using Wpf.Application.DTOs;
using Wpf.Domain.Entities;

namespace Wpf.Infrastructure.Mapping;

[Mapper]
public partial class CustomerMapper
{
    public partial CustomerDto ToDto(Customer entity);
    public partial List<CustomerDto> ToDtoList(List<Customer> entities);
}