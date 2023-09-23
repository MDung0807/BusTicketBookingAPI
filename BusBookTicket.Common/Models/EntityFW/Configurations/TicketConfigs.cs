﻿using BusBookTicket.Common.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusBookTicket.Common.Models.EntityFW.Configurations
{
    public class TicketConfigs : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {

            #region -- Properties --
            builder.HasKey(x => x.ticketID);

            builder.Property(x => x.ticketID)
                .ValueGeneratedOnAdd();
            #endregion -- Properties --

            #region -- Relationship --
            builder.HasOne(x => x.busStation)
                .WithMany(x => x.tickets)
                .HasForeignKey("busStationStartID")
                .IsRequired();
            builder.HasOne(x => x.busStation)
                .WithMany(x => x.tickets)
                .HasForeignKey("busStationEndID")
                .IsRequired();

            builder.HasOne(x => x.customer)
               .WithMany(x => x.tickets)
               .HasForeignKey("customerID")
               .IsRequired();

            builder.HasOne(x => x.discount)
               .WithMany(x => x.tickets)
               .HasForeignKey("discountID")
               .IsRequired() ;
            #endregion -- Relationship --
        }
    }
}
