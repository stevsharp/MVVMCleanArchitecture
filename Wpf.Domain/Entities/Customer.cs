using SharedKernel;

using Wpf.Domain.Abstractions;
using Wpf.Domain.Common;
namespace Wpf.Domain.Entities;

public class Customer : BaseEntity<int>, IAggregateRoot
{
    public string Name { get; protected set; } = string.Empty;

    public Customer(string name)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        this.Name = name;
    }

    public ICollection<Order> Orders { get; set; } = [];

    public void AddOrder(string description)
    {
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));

        Orders.Add(new Order
        {
            Description = description,
            CustomerId = this.Id
        });
    }

    public void Rename(string name)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        this.Name = name;   
    }
}
