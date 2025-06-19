
namespace Wpf.Domain.Abstractions;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}
