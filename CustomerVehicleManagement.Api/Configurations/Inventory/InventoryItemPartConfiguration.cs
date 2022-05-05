﻿using CustomerVehicleManagement.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Configurations.Inventory
{
    public class InventoryItemPartConfiguration : EntityConfiguration<InventoryItemPart>
    {
        public override void Configure(EntityTypeBuilder<InventoryItemPart> builder)
        {
            base.Configure(builder);
            builder.ToTable("InventoryItemPart", "dbo");

            builder.Ignore(item => item.TrackingState);
        }
    }
}
