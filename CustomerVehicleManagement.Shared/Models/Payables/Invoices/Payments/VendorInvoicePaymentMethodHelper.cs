﻿using CustomerVehicleManagement.Domain.Entities.Payables;
using CustomerVehicleManagement.Shared.Models.Payables.Vendors;
using System;

namespace CustomerVehicleManagement.Shared.Models.Payables.Invoices.Payments
{
    public class VendorInvoicePaymentMethodHelper
    {
        public static VendorInvoicePaymentMethodToWrite ConvertReadToWriteDto(VendorInvoicePaymentMethodToRead payMethod)
        {
            if (payMethod is null)
                return null;

            return new()
            {
                Name = payMethod.Name,
                IsActive = payMethod.IsActive,
                //IsOnAccountPaymentType = payMethod.IsOnAccountPaymentType,
                PaymentType = payMethod.PaymentType,
                ReconcilingVendor = payMethod.ReconcilingVendor
            };
        }

        public static VendorInvoicePaymentMethodToRead ConvertEntityToReadDto(VendorInvoicePaymentMethod payMethod)
        {
            return payMethod is null
                ? null
                : new()
                {
                    Id = payMethod.Id,
                    Name = payMethod.Name,
                    IsActive = payMethod.IsActive,
                    //IsOnAccountPaymentType = payMethod.IsOnAccountPaymentType,
                    PaymentType = payMethod.PaymentType,
                    ReconcilingVendor = VendorHelper.ConvertEntityToReadDto(payMethod.ReconcilingVendor)
                };
        }

        public static VendorInvoicePaymentMethodToReadInList ConvertEntityToReadInListDto(VendorInvoicePaymentMethod payMethod)
        {
            return payMethod is null
                ? null
                : new()
                {
                    Id = payMethod.Id,
                    Name = payMethod.Name,
                    IsActive = payMethod.IsActive,
                    PaymentType = payMethod.PaymentType,
                    ReconcilingVendorName = payMethod.ReconcilingVendor?.Name ?? "N/A"
                };
        }

        internal static VendorInvoicePaymentMethodToRead ConvertWriteToReadDto(VendorInvoicePaymentToWrite payment, VendorInvoicePaymentMethodToRead method)
        {
            if (payment.PaymentMethod.Id == method.Id && payment is not null)
                return
                    new VendorInvoicePaymentMethodToRead()
                    {
                        Id = payment.PaymentMethod.Id,
                        Name = method.Name,
                        IsActive = method.IsActive,
                        //IsOnAccountPaymentType = method.IsOnAccountPaymentType,
                        PaymentType = method.PaymentType,
                        ReconcilingVendor = method.ReconcilingVendor
                    };

            throw new ArgumentException("Unable to ConvertWriteToReadDto");
        }

        public static VendorInvoicePaymentMethodToRead ConvertReadInListToReadDto(VendorInvoicePaymentMethodToReadInList paymentMethod)
        {
            return
            paymentMethod is null
                ? null
                : new()
                {
                    Id = paymentMethod.Id,
                    Name = paymentMethod.Name,
                    IsActive = paymentMethod.IsActive,
                    PaymentType = paymentMethod.PaymentType
                    //ReconcilingVendor = paymentMethod.ReconcilingVendorName
                };
        }
    }
}
