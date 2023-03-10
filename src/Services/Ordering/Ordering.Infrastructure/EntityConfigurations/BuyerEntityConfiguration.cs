﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.EntityConfigurations;

internal class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> buyerConfiguration)
    {
        buyerConfiguration.ToTable("buyers", OrderDbContext.DEFAULT_SCHEMA);

        buyerConfiguration.HasKey(b => b.Id);

        buyerConfiguration.Ignore(b => b.DomainEvents);
        buyerConfiguration.Property(b => b.Id).ValueGeneratedOnAdd();

        buyerConfiguration.Property(b => b.Name).HasColumnType("name").HasColumnType("varchar").HasMaxLength(100);

        buyerConfiguration.HasMany(b => b.PaymentMethods).WithOne().HasForeignKey(i => i.Id).OnDelete(DeleteBehavior.Cascade);

        var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
