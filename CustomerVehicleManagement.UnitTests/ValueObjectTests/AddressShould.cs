﻿using CustomerVehicleManagement.Shared.TestUtilities;
using FluentAssertions;
using Menominee.Common.Enums;
using Menominee.Common.ValueObjects;
using System;
using Xunit;

namespace CustomerVehicleManagement.UnitTests.ValueObjectTests
{
    public class AddressShould
    {
        [Fact]
        public void Create_Address()
        {
            var addressLine = "1234 Five Street";
            var city = "Gaylord";
            var state = State.MI;
            var postalCode = "49735";

            var addressOrError = Address.Create(addressLine, city, state, postalCode);

            addressOrError.Should().NotBeNull();
            addressOrError.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Empty_AddressLine()
        {
            string addressLine = null;
            var city = "Gaylord";
            var state = State.MI;
            var postalCode = "49735";

            var addressOrError = Address.Create(addressLine, city, state, postalCode);

            addressOrError.IsFailure.Should().BeTrue();
            addressOrError.Error.Should().Be(Address.AddressUnderMinimumLengthMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Empty_City()
        {
            var addressLine = "1234 Five Street";
            string city = null;
            var state = State.MI;
            var postalCode = "49735";

            var addressOrError = Address.Create(addressLine, city, state, postalCode);

            addressOrError.IsFailure.Should().BeTrue();
            addressOrError.Error.Should().Be(Address.CityUnderMinimumLengthMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Empty_PostalCode()
        {
            var addressLine = "1234 Five Street";
            var city = "Gaylord";
            var state = State.MI;
            string postalCode = null;

            var addressOrError = Address.Create(addressLine, city, state, postalCode);

            addressOrError.IsFailure.Should().BeTrue();
            addressOrError.Error.Should().Be(Address.PostalCodeUnderMinimumLengthMessage);
        }

        [Fact]
        public void Equate_Two_Address_Instances_Having_Same_Values()
        {
            var address1 = Utilities.CreateValidAddress();
            var address2 = Utilities.CreateValidAddress();

            address1.Should().BeEquivalentTo(address2);
        }

        [Fact]
        public void Not_Equate_Two_Address_Instances_Having_Differing_Values()
        {
            var address1 = Utilities.CreateValidAddress();
            var address2 = Utilities.CreateValidAddress();
            var newAddressLine = "54321";

            address2 = address2.NewAddressLine(newAddressLine);

            address1.Should().NotBeEquivalentTo(address2);
        }


        [Fact]
        public void Return_New_Address_On_NewAddressLine()
        {
            var address = Utilities.CreateValidAddress();
            var newAddressLine = "5432 One Street";

            address = address.NewAddressLine("5432 One Street");

            address.AddressLine.Should().Be(newAddressLine);
        }

        [Fact]
        public void Throw_Exception_On_NewAddressLine_Passing_Null_Parameter()
        {
            var address = Utilities.CreateValidAddress();

            Action action = () => address = address.NewAddressLine(null);

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Return_New_Address_On_NewCity()
        {
            var address = Utilities.CreateValidAddress();
            var newCity = "Oomapopalis";

            address = address.NewCity(newCity);

            address.City.Should().Be(newCity);
        }

        [Fact]
        public void Throw_Exception_On_NewCity_Passing_Null_Parameter()
        {
            var address = Utilities.CreateValidAddress();

            Action action = () => address = address.NewCity(null);

            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Return_New_Address_On_NewState()
        {
            var address = Utilities.CreateValidAddress();
            var newState = State.HI;
            address = address.NewState(newState);

            address.State.Should().Be(newState);
        }

        [Fact]
        public void Return_New_Address_On_NewPostalCode()
        {
            var address = Utilities.CreateValidAddress();
            var newPostalCode = "55555";

            address = address.NewPostalCode(newPostalCode);

            address.PostalCode.Should().Be(newPostalCode);
        }

        [Fact]
        public void Throw_Exception_On_NewPostalCode_Passing_Null_Parameter()
        {
            var address = Utilities.CreateValidAddress();

            Action action = () => address = address = address.NewPostalCode(null);

            action.Should().Throw<Exception>();
        }
    }
}
