
namespace Wpf.Application.DTOs;

public record OrderDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CustomerId { get; set; }
}
