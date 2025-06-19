

namespace Wpf.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
