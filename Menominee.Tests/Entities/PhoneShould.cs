﻿using FluentAssertions;
using Menominee.Domain.Entities;
using Menominee.Domain.Enums;
using Xunit;

namespace Menominee.Tests.Entities
{
    public class PhoneShould
    {
        [Fact]
        public void Create_Phone()
        {
            // Arrange
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;

            // Act
            var phoneOrError = Phone.Create(number, phoneType, true);

            // Assert
            phoneOrError.Value.Should().BeOfType<Phone>();
            phoneOrError.IsFailure.Should().BeFalse();
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Null_Number()
        {
            string number = null;
            var phoneType = PhoneType.Home;

            var result = Phone.Create(number, phoneType, true);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Empty_Number()
        {
            string number = string.Empty;
            var phoneType = PhoneType.Home;

            var result = Phone.Create(number, phoneType, true);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Invalid_Number()
        {
            var number = "989.627.9206?";
            var phoneType = PhoneType.Home;

            var result = Phone.Create(number, phoneType, true);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Invalid_PhoneType()
        {
            var number = "989.627.9206";
            var phoneType = (PhoneType)99;

            var result = Phone.Create(number, phoneType, true);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.PhoneTypeInvalidMessage);
        }

        [Fact]
        public void SetNumber()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;

            var newNumber = "555-555-5555";

            phone.Number.Should().Be(number);
            phone.SetNumber(newNumber);

            phone.Number.Should().Be(newNumber);
        }

        [Fact]
        public void Return_Failure_On_Set_Invalid_Number()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;
            var invalidNumber = "555-555-5555?";

            var result = phone.SetNumber(invalidNumber);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void SetPhoneType()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;

            phone.PhoneType.Should().Be(PhoneType.Home);
            phone.SetPhoneType(PhoneType.Mobile);

            phone.PhoneType.Should().Be(PhoneType.Mobile);
        }

        [Fact]
        public void Return_Failure_On_Set_Invalid_PhoneType()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;
            var invalidPhoneType = (PhoneType)(-1);
            phone.PhoneType.Should().Be(PhoneType.Home);

            var result = phone.SetPhoneType(invalidPhoneType);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void SetIsPrimary()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;

            phone.IsPrimary.Should().BeTrue();
            phone.SetIsPrimary(false);

            phone.IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void ReturnFormattedTenDigitPhoneNumberOnToString()
        {
            var number = "5551234567";
            var numberFormatted = "(555) 123-4567";
            var phoneType = PhoneType.Home;

            var phone = Phone.Create(number, phoneType, true).Value;

            phone.ToString().Should().Be(numberFormatted);
        }

        [Fact]
        public void ReturnFormattedSevenDigitPhoneNumberOnToString()
        {
            var number = "6279206";
            var numberFormatted = "627-9206";
            var phoneType = PhoneType.Home;

            var phone = Phone.Create(number, phoneType, true).Value;

            phone.ToString().Should().Be(numberFormatted);
        }

        [Fact]
        public void ReturnNonFormattedPhoneNumberOnToStringForNonStandardLengthNumbers()
        {
            var number = "896279206";
            var phoneType = PhoneType.Home;

            var phone = Phone.Create(number, phoneType, true).Value;

            phone.ToString().Should().Be("896279206");

            phone.SetNumber("96279206");

            phone.ToString().Should().Be("96279206");

            phone.SetNumber("279206");

            phone.ToString().Should().Be("279206");

            phone.SetNumber("79206");

            phone.ToString().Should().Be("79206");

            phone.SetNumber("9206");

            phone.ToString().Should().Be("9206");

            phone.SetNumber("206");

            phone.ToString().Should().Be("206");

            phone.SetNumber("06");

            phone.ToString().Should().Be("06");

            phone.SetNumber("6");

            phone.ToString().Should().Be("6");
        }

        [Fact]
        public void Not_Set_Null_Number()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;
            string nullNumber = null;

            var result = phone.SetNumber(nullNumber);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_Failure_On_Set_Empty_Number()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;
            var emptyNumber = string.Empty;

            var result = phone.SetNumber(emptyNumber);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_Failure_On_Set_Undefined_PhoneType()
        {
            var number = "989.627.9206";
            var phoneType = PhoneType.Home;
            var phone = Phone.Create(number, phoneType, true).Value;
            var undefinedPhoneType = (PhoneType)99;

            var result = phone.SetPhoneType(undefinedPhoneType);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

        [Fact]
        public void Return_Failure_On_Create_With_Whitespace_Number()
        {
            var number = "    ";
            var phoneType = PhoneType.Home;

            var result = Phone.Create(number, phoneType, true);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Phone.InvalidMessage);
        }

    }
}
