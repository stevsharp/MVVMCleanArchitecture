using Wpf.Domain.Common;
using Wpf.Domain.Entities;

namespace Wpf.Tests.Domain;

public class CustomerTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_rejects_blank_name(string name)
    {
        Assert.Throws<ArgumentException>(() => new Customer(name));
    }

    [Fact]
    public void Rename_changes_name()
    {
        var customer = new Customer("Alice");

        customer.Rename("Alicia");

        Assert.Equal("Alicia", customer.Name);
    }

    [Fact]
    public void Rename_rejects_blank_name()
    {
        var customer = new Customer("Alice");

        Assert.Throws<ArgumentException>(() => customer.Rename(" "));
        Assert.Equal("Alice", customer.Name);
    }

    [Fact]
    public void AddOrder_appends_order_with_description()
    {
        var customer = new Customer("Alice");

        customer.AddOrder("Order A1");

        var order = Assert.Single(customer.Orders);
        Assert.Equal("Order A1", order.Description);
    }

    [Fact]
    public void AddOrder_rejects_blank_description()
    {
        var customer = new Customer("Alice");

        Assert.Throws<ArgumentException>(() => customer.AddOrder(""));
        Assert.Empty(customer.Orders);
    }

    [Fact]
    public void Guard_AgainstNegative_rejects_negative_values()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Guard.AgainstNegative(-1m, "amount"));
        Guard.AgainstNegative(0m, "amount");
    }
}
