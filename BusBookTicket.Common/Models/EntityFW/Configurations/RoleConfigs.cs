﻿using BusBookTicket.Common.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookTicket.Common.Models.EntityFW.Configurations
{
    public class RoleConfigs : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            #region -- Properties --
            builder.HasKey(x => x.roleID);

            builder.Property(x => x.roleID).ValueGeneratedOnAdd();
            builder.HasAlternateKey(x => x.roleName);
            #endregion -- Properties --

            #region -- Relationship --
            
            #endregion -- Relationship --
        }

    }
}
