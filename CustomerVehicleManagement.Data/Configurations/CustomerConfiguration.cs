﻿using CustomerVehicleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerVehicleManagement.Data.Configurations
{
    public class CustomerConfiguration : EntityConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder); // <--
            builder.ToTable("Customer", "dbo");
            builder.Ignore(customer => customer.Entity);
            builder.Ignore(customer => customer.TrackingState);
            builder.Ignore(customer => customer.Phones);
            builder.Ignore(customer => customer.Emails);

            // Value Object: ContactPreferences
            builder.OwnsOne(customer => customer.ContactPreferences)
                   .Property(contactPreferences => contactPreferences.AllowEmail)
                   .HasColumnName("AllowEmail")
                   .IsRequired();
            builder.OwnsOne(customer => customer.ContactPreferences)
                   .Property(contactPreferences => contactPreferences.AllowMail)
                   .HasColumnName("AllowMail")
                   .IsRequired();
            builder.OwnsOne(customer => customer.ContactPreferences)
                   .Property(contactPreferences => contactPreferences.AllowSms)
                   .HasColumnName("AllowSms")
                   .IsRequired();
        }
    }
}
