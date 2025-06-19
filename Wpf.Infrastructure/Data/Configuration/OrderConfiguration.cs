using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wpf.Domain.Entities;

namespace Wpf.Infrastructure.Data;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
               .IsRequired()
               .HasMaxLength(500);
    }
}
