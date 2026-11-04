using Riok.Mapperly.Abstractions;

using Wpf.Application.DTOs;
using Wpf.Domain.Entities;

namespace Wpf.Application.Mapping;

[Mapper]
public partial class OrderMapper
{
    [MapperIgnoreSource(nameof(Order.Customer))]
    public partial OrderDto ToDto(Order entity);

    public partial List<OrderDto> ToDtoList(IEnumerable<Order> entities);
}
