using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.EntityConfigurations;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Ignore(i => i.DomainEvents);

        builder
            .OwnsOne(
                o => o.Address,
                a =>
                {
                    a.WithOwner();
                });

        builder
            .Property<int>("orderStatusId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("OrderStatusId")
            .IsRequired();


        var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(i => i.BuyerId);


        builder.HasOne(o => o.OrderStatus).WithMany().HasForeignKey("orderStatusId");
    }
}
