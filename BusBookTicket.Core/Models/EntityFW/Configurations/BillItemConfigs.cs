﻿using BusBookTicket.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusBookTicket.Core.Models.EntityFW.Configurations
{
    public class BillItemConfigs : IEntityTypeConfiguration<BillItem>
    {
        public void Configure(EntityTypeBuilder<BillItem> builder)
        {

            #region -- Properties --
            builder.HasKey(x => x.billItemID);

            builder.Property(x => x.billItemID)
                .ValueGeneratedOnAdd();
            #endregion -- Properties --

            #region -- Relationship --
            builder.HasOne(x => x.bill)
                .WithMany(x => x.billItems)
                .HasForeignKey("ticketID")
                .IsRequired();

            builder.HasOne(x => x.seat)
                .WithOne(x => x.ticketItem)
                .HasForeignKey<BillItem>("seatID")
                .IsRequired();
            #endregion -- Relationship --
        }
    }
}