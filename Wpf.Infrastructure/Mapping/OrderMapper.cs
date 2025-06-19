using Riok.Mapperly.Abstractions;


using Wpf.Application.DTOs;
using Wpf.Domain.Entities;

namespace Wpf.Infrastructure.Mapping;

[Mapper]
public partial class OrderMapper
{
    public partial OrderDto ToDto(Order entity);
    public partial List<OrderDto> ToDtoList(List<Order> entities);
}
