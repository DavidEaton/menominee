﻿using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Domain.Entities.Payables;
using FluentAssertions;
using Menominee.Common.Enums;
using Menominee.Common.ValueObjects;
using System.Collections.Generic;
using Xunit;
using static CustomerVehicleManagement.Tests.Utilities;

namespace CustomerVehicleManagement.Tests.Unit.Entities
{
    public class VendorShould
    {
        [Fact]
        public void Create_Vendor()
        {
            // Arrange
            var name = "Vendor One";
            var vendorCode = "V1";

            // Act
            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            // Assert
            vendorOrError.IsFailure.Should().BeFalse();
            vendorOrError.Value.Should().BeOfType<Vendor>();
            vendorOrError.Value.Name.Should().Be(name);
            vendorOrError.Value.VendorCode.Should().Be(vendorCode);
        }

        [Fact]
        public void Create_Vendor_With_Optional_DefaultPaymentMethod()
        {
            var name = "Vendor One";
            var vendorCode = "V1";
            var paymentMethod = CreateVendorInvoicePaymentMethod();
            var defaultPaymentMethod = DefaultPaymentMethod.Create(paymentMethod, true).Value;

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier, defaultPaymentMethod);

            vendorOrError.IsFailure.Should().BeFalse();
            vendorOrError.Value.Should().BeOfType<Vendor>();
            vendorOrError.Value.DefaultPaymentMethod.Should().BeOfType<DefaultPaymentMethod>();
            vendorOrError.Value.DefaultPaymentMethod.Should().Be(defaultPaymentMethod);
            vendorOrError.Value.VendorCode.Should().Be(vendorCode);
        }

        [Fact]
        public void Create_Vendor_With_Null_DefaultPaymentMethod()
        {
            var name = "Vendor One";
            var vendorCode = "V1";
            DefaultPaymentMethod defaultPaymentMethod = null;

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier, defaultPaymentMethod);

            vendorOrError.IsFailure.Should().BeFalse();
            vendorOrError.Value.Should().BeOfType<Vendor>();
            vendorOrError.Value.DefaultPaymentMethod.Should().Be(null);
            vendorOrError.Value.VendorCode.Should().Be(vendorCode);
        }

        [Fact]
        public void Create_Vendor_With_Optional_Emails()
        {
            var name = "Vendor One";
            var vendorCode = "V1";
            var phones = new List<Phone>();
            var number = "555.444.3333";
            var phoneType = PhoneType.Mobile;
            var phone0 = Phone.Create(number, phoneType, true).Value;
            phones.Add(phone0);
            number = "231.546.2102";
            phoneType = PhoneType.Home;
            var phone1 = Phone.Create(number, phoneType, false).Value;
            phones.Add(phone1);

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            vendorOrError.IsFailure.Should().BeFalse();
            vendorOrError.Value.Should().BeOfType<Vendor>();
            vendorOrError.Value.VendorCode.Should().Be(vendorCode);
        }

        [Fact]
        public void Create_Vendor_With_Optional_Phones()
        {
            var name = "Vendor One";
            var vendorCode = "V1";
            var phones = CreateTestPhones(10);

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier, phones: phones);

            vendorOrError.IsFailure.Should().BeFalse();
            vendorOrError.Value.Should().BeOfType<Vendor>();
            vendorOrError.Value.Phones.Count.Should().BeGreaterThan(1);
        }


        [Fact]
        public void Not_Create_Vendor_With_Null_Name()
        {
            var vendorCode = RandomCharacters(Vendor.MinimumLength);
            string name = null;

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            vendorOrError.IsFailure.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestData.Data), MemberType = typeof(TestData))]
        public void Not_Create_Vendor_With_Invalid_Name(int length)
        {
            var name = RandomCharacters(length);
            var vendorCode = RandomCharacters(Vendor.MinimumLength);

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            vendorOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Not_Create_Vendor_With_Null_Code()
        {
            var name = RandomCharacters(Vendor.MinimumLength);
            string code = null;

            var vendorOrError = Vendor.Create(name, code, VendorRole.PartsSupplier);

            vendorOrError.IsFailure.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestData.Data), MemberType = typeof(TestData))]
        public void Not_Create_Vendor_With_Invalid_Code(int length)
        {
            var name = RandomCharacters(Vendor.MinimumLength);
            var vendorCode = RandomCharacters(length);

            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            vendorOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void SetName()
        {
            var vendor = CreateVendor();

            var name = RandomCharacters(Vendor.MinimumLength + 11);
            vendor.SetName(name);

            vendor.Name.Should().Be(name);
        }

        [Fact]
        public void Not_Set_Name_With_Null_Name()
        {
            var vendorOrError = CreateVendor();

            var resultOrError = vendorOrError.SetName(null);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestData.Data), MemberType = typeof(TestData))]
        public void Not_Set_Name_With_Invalid_Name(int length)
        {
            var name = RandomCharacters(Vendor.MinimumLength);
            var vendorCode = RandomCharacters(Vendor.MinimumLength);
            var vendorOrError = Vendor.Create(name, vendorCode, VendorRole.PartsSupplier);

            var invalidName = RandomCharacters(length);
            var resultOrError = vendorOrError.Value.SetName(invalidName);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void SetVendorCode()
        {
            var vendor = CreateVendor();
            vendor.VendorCode.Length.Should().Be(Vendor.MinimumLength);
            var vendorCode = RandomCharacters(Vendor.MinimumLength + 1);

            vendor.SetVendorCode(vendorCode);

            vendor.VendorCode.Should().Be(vendorCode);
        }

        [Fact]
        public void SetDefaultPaymentMethod()
        {
            var vendor = CreateVendor();
            var paymentMethod = CreateVendorInvoicePaymentMethod();
            var defaultPaymentMethod = DefaultPaymentMethod.Create(paymentMethod, true).Value;

            vendor.SetDefaultPaymentMethod(defaultPaymentMethod);

            vendor.DefaultPaymentMethod.Should().Be(defaultPaymentMethod);
        }

        [Fact]
        public void Not_SetDefaultPaymentMethod_With_Null_DefaultPaymentMethod()
        {
            var vendor = CreateVendor();
            var paymentMethod = CreateVendorInvoicePaymentMethod();
            DefaultPaymentMethod defaultPaymentMethod = null;

            var resultOrError = vendor.SetDefaultPaymentMethod(defaultPaymentMethod);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void ClearDefaultPaymentMethod()
        {
            var vendor = CreateVendor();
            var paymentMethod = CreateVendorInvoicePaymentMethod();
            var defaultPaymentMethod = DefaultPaymentMethod.Create(paymentMethod, true).Value;
            vendor.SetDefaultPaymentMethod(defaultPaymentMethod);
            vendor.DefaultPaymentMethod.Should().Be(defaultPaymentMethod);

            var resultOrError = vendor.ClearDefaultPaymentMethod();

            resultOrError.IsFailure.Should().BeFalse();
            vendor.DefaultPaymentMethod.Should().Be(null);
        }

        [Theory]
        [MemberData(nameof(TestData.Data), MemberType = typeof(TestData))]
        public void Not_Set_Invalid_Vendor_Code(int length)
        {
            var vendor = CreateVendor();

            var invalidVendorCode = RandomCharacters(length);
            var resultOrError = vendor.SetVendorCode(invalidVendorCode);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Not_Set_Null_Vendor_Code()
        {
            var vendor = CreateVendor();

            var resultOrError = vendor.SetVendorCode(null);

            resultOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void Disable()
        {
            var vendor = CreateVendor();

            vendor.Disable();

            vendor.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Enable()
        {
            var vendor = CreateVendor();

            vendor.Disable();
            vendor.IsActive.Should().BeFalse();

            vendor.Enable();
            vendor.IsActive.Should().BeTrue();
        }

        [Fact]
        public void AddPhone()
        {
            var vendor = CreateVendor();
            var number = "555.444.3333";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;

            vendor.AddPhone(phone);

            vendor.Phones.Should().Contain(phone);
        }

        [Fact]
        public void Not_AddPhone_If_Not_Unique()
        {
            var vendor = CreateVendor();
            var number = "555.444.3333";
            var phoneType = PhoneType.Home;
            var phoneOne = Phone.Create(number, phoneType, true).Value;
            var numberTwo = "555.444.3333";
            var phoneTypeTwo = PhoneType.Work;
            var phoneTwo = Phone.Create(numberTwo, phoneTypeTwo, true).Value;

            vendor.AddPhone(phoneOne);
            var vendorOrError = vendor.AddPhone(phoneTwo);

            vendor.Phones.Should().Contain(phoneOne);
            vendorOrError.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void ClearAddress()
        {
            var vendor = CreateVendor();
            var address = Address.Create(
                "1234 Five Street",
                "Gaylord",
                State.MI,
                "49735").Value;
            vendor.SetAddress(address);
            vendor.Address.Should().Be(address);

            vendor.ClearAddress();

            vendor.Address.Should().BeNull();
        }

        [Fact]
        public void SetAddress()
        {
            var vendor = CreateVendor();
            var address = Address.Create(
                "1234 Five Street",
                "Gaylord",
                State.MI,
                "49735").Value;

            vendor.SetAddress(address);
            vendor.Address.Should().Be(address);
        }

        [Fact]
        public void SetVendorRole()
        {
            var vendor = CreateVendor();
            vendor.VendorRole.Should().Be(VendorRole.PartsSupplier);
            var newVendorRole = VendorRole.PaymentReconciler;

            vendor.SetVendorRole(newVendorRole);

            vendor.VendorRole.Should().Be(newVendorRole);
        }

        internal class TestData
        {
            public static IEnumerable<object[]> Data
            {
                get
                {
                    yield return new object[] { Vendor.MinimumLength - 1 };
                    yield return new object[] { Vendor.MaximumLength + 1 };
                }
            }
        }
    }
}
