

using Wpf.Domain.Entities;

namespace Wpf.Infrastructure.Data;

public static class AppDbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Customers.Any())
            return; 

        var customers = new List<Customer>
        {
            new("Alice"),
            new("Bob"),
            new("Charlie")
        };

        customers[0].AddOrder("Order A1");
        customers[0].AddOrder("Order A2");

        customers[1].AddOrder("Order B1");

        db.Customers.AddRange(customers);
        db.SaveChanges();
    }
}
