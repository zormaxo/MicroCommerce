﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.EntityConfigurations;

class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> orderItemConfiguration)
    {
        orderItemConfiguration.ToTable("orderItems", OrderDbContext.DEFAULT_SCHEMA);

        orderItemConfiguration.HasKey(o => o.Id);

        orderItemConfiguration.Ignore(b => b.DomainEvents);

        orderItemConfiguration.Property(o => o.Id).ValueGeneratedOnAdd();

        orderItemConfiguration.Property<int>("OrderId").IsRequired();
    }
}
