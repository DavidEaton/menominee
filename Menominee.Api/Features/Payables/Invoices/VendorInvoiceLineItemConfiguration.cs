﻿using Menominee.Domain.Entities.Payables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menominee.Api.Features.Payables.Invoices
{
    public class VendorInvoiceLineItemConfiguration : EntityConfiguration<VendorInvoiceLineItem>
    {
        public override void Configure(EntityTypeBuilder<VendorInvoiceLineItem> builder)
        {
            base.Configure(builder);
            builder.ToTable("VendorInvoiceLineItem", "dbo");

            builder.Property(lineItem => lineItem.Type)
                .IsRequired();

            builder.Property(lineItem => lineItem.Quantity)
                .IsRequired();

            // Value Object: VendorInvoiceItem
            // Configure Value Object to contain two Entity references
            builder.OwnsOne(
                lineItem => lineItem.Item, builder =>
                {
                    builder.WithOwner();
                    builder.Navigation(item => item.Manufacturer)
                        .UsePropertyAccessMode(PropertyAccessMode.Property);
                    builder.Navigation(item => item.SaleCode)
                        .UsePropertyAccessMode(PropertyAccessMode.Property);
                });

            builder.OwnsOne(lineItem => lineItem.Item)
                .Property(item => item.PartNumber)
                .HasColumnName("ItemPartNumber")
                .HasMaxLength(255)
                .IsRequired();

            builder.OwnsOne(lineItem => lineItem.Item)
               .Property(item => item.Description)
               .HasColumnName("ItemDescription")
               .HasMaxLength(255)
                .IsRequired();
        }
    }
}
