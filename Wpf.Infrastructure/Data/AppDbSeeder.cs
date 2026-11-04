using Microsoft.EntityFrameworkCore;

using Wpf.Domain.Entities;

namespace Wpf.Infrastructure.Data;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Customers.AnyAsync(cancellationToken))
            return;

        var alice = new Customer("Alice");
        alice.AddOrder("Order A1");
        alice.AddOrder("Order A2");

        var bob = new Customer("Bob");
        bob.AddOrder("Order B1");

        var charlie = new Customer("Charlie");

        db.Customers.AddRange(alice, bob, charlie);
        await db.SaveChangesAsync(cancellationToken);
    }
}
