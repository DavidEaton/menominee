﻿using System.ComponentModel.DataAnnotations;

namespace Menominee.Domain.Enums
{
    public enum VendorRole
    {
        [Display(Name = "Parts Supplier")]
        PartsSupplier,
        [Display(Name = "Payment Reconciler")]
        PaymentReconciler,
        [Display(Name = "Non-Parts Supplier")]
        NonPartsSupplier
    }
}
