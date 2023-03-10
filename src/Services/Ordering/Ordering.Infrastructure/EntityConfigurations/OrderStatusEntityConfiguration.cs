﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.EntityConfigurations;

internal class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("orderstatus", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Id).HasDefaultValue(1).ValueGeneratedNever().IsRequired();

        builder.Property(o => o.Name).HasMaxLength(200).IsRequired();
    }
}
