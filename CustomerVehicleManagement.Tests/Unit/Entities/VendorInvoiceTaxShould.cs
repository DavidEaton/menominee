﻿using CustomerVehicleManagement.Domain.Entities.Payables;
using CustomerVehicleManagement.Domain.Entities.Taxes;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using static CustomerVehicleManagement.Tests.Utilities;

namespace CustomerVehicleManagement.Tests.Unit.Entities
{
    public class VendorInvoiceTaxShould
    {
        private readonly double validAmount = 6.50;
        [Fact]
        public void Create_VendorInvoiceTax()
        {
            // Arrange
            var salesTax = CreateSalesTax();

            // Act
            var vendorInvoiceTaxOrError = VendorInvoiceTax.Create(salesTax, validAmount);

            // Assert
            vendorInvoiceTaxOrError.IsFailure.Should().BeFalse();
            vendorInvoiceTaxOrError.Value.Should().BeOfType<VendorInvoiceTax>();
        }

        [Fact]
        public void Not_Create_VendorInvoiceTax_With_Null_SalesTax()
        {
            SalesTax nullSalesTax = null;

            var vendorInvoiceTaxOrError = VendorInvoiceTax.Create(nullSalesTax, validAmount);

            vendorInvoiceTaxOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Not_Create_VendorInvoiceTax_With_Invalid_Amount()
        {
            double invalidAmount = -0.1;

            var vendorInvoiceTaxOrError = VendorInvoiceTax.Create(CreateSalesTax(), invalidAmount);

            vendorInvoiceTaxOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void SetSalesTax()
        {
            var vendorInvoiceTax = VendorInvoiceTax.Create(CreateSalesTax(), validAmount).Value;

            var salesTax = CreateSalesTax(1);
            vendorInvoiceTax.SetSalesTax(salesTax);

            vendorInvoiceTax.SalesTax.Should().Be(salesTax);
        }

        [Fact]
        public void Not_Set_Null_SalesTax()
        {
            var vendorInvoiceTax = VendorInvoiceTax.Create(CreateSalesTax(), validAmount).Value;

            var resultOrError = vendorInvoiceTax.SetSalesTax(null);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void SetAmount()
        {
            var vendorInvoiceTax = VendorInvoiceTax.Create(CreateSalesTax(), validAmount).Value;

            vendorInvoiceTax.Amount.Should().Be(validAmount);
            double newAmount = 2.1;
            vendorInvoiceTax.SetAmount(newAmount);

            vendorInvoiceTax.Amount.Should().Be(newAmount);
        }

        [Fact]
        public void Not_Set_Invalid_Amount()
        {
            var vendorInvoiceTax = VendorInvoiceTax.Create(CreateSalesTax(), validAmount).Value;

            vendorInvoiceTax.Amount.Should().Be(validAmount);
            double invalidAmount = -0.1;
            var resultOrError = vendorInvoiceTax.SetAmount(invalidAmount);

            resultOrError.IsFailure.Should().BeTrue();
        }

        internal class TestData
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { (long)int.MaxValue + 1 };
                    yield return new object[] { (long)int.MinValue - 1 };
                }
            }
        }
    }
}
