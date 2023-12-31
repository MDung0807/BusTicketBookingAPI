﻿using BusBookTicket.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusBookTicket.Core.Models.EntityFW.Configurations;

public class PriceClassificationConfig : IEntityTypeConfiguration<PriceClassification>
{
    public void Configure(EntityTypeBuilder<PriceClassification> builder)
    {
        #region -- Relationship --

        builder.HasOne(x => x.Company)
            .WithMany(x => x.PriceClassifications)
            .HasForeignKey("CompanyId");

        #endregion -- Relationship --
    }
}