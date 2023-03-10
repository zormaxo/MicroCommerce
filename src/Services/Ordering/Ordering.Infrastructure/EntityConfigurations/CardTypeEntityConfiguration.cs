﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Infrastructure.Context;

namespace Ordering.Infrastructure.EntityConfigurations;

internal class CardTypeEntityConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypesConfiguration)
    {
        cardTypesConfiguration.ToTable("cardtypes", OrderDbContext.DEFAULT_SCHEMA);

        cardTypesConfiguration.HasKey(ct => ct.Id);
        cardTypesConfiguration.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();

        cardTypesConfiguration.Property(ct => ct.Id).HasDefaultValue(1).ValueGeneratedNever().IsRequired();

        cardTypesConfiguration.Property(ct => ct.Name).HasMaxLength(200).IsRequired();
    }
}
