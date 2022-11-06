﻿using CSharpFunctionalExtensions;
using CustomerVehicleManagement.Shared;
using FluentAssertions;
using Menominee.Common.ValueObjects;
using Xunit;

namespace CustomerVehicleManagement.Tests.Unit.ValueObjects
{
    public class OrganizationNameShould
    {
        [Fact]
        public void Create_OrganizationName()
        {
            var name = "jane's";

            var organizationNameOrError = OrganizationName.Create(name);

            organizationNameOrError.IsSuccess.Should().BeTrue();
            organizationNameOrError.Value.Name.Should().Be(name);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Empty_Name()
        {
            var name = string.Empty;

            var organizationNameOrError = OrganizationName.Create(name);

            organizationNameOrError.IsFailure.Should().BeTrue();
            organizationNameOrError.Error.Should().Contain(OrganizationName.InvalidMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_With_Null_Name()
        {
            string name = null;

            var organizationNameOrError = OrganizationName.Create(name);

            organizationNameOrError.IsFailure.Should().BeTrue();
            organizationNameOrError.Error.Should().Contain(OrganizationName.InvalidMessage);
        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_When_Exceeds_Maximum_Length()
        {
            string name = Utilities.LoremIpsum(OrganizationName.MaximumLength + 1);

            var organizationNameOrError = OrganizationName.Create(name);

            organizationNameOrError.IsFailure.Should().BeTrue();
            organizationNameOrError.Error.Should().Contain(OrganizationName.InvalidMessage);

        }

        [Fact]
        public void Return_IsFailure_Result_On_Create_When_Under_Minimum_Length()
        {
            string name = Utilities.LoremIpsum(OrganizationName.MinimumLength - 1);

            var organizationNameOrError = OrganizationName.Create(name);

            organizationNameOrError.IsFailure.Should().BeTrue();
            organizationNameOrError.Error.Should().Contain(OrganizationName.InvalidMessage);

        }

        [Fact]
        public void Equate_Two_OrganizationName_Instances_Having_Same_Values()
        {
            var name1 = "june's";
            var name2 = "june's";

            var organizationNameOrError1 = OrganizationName.Create(name1);
            var organizationNameOrError2 = OrganizationName.Create(name2);


            organizationNameOrError1.Should().BeEquivalentTo(organizationNameOrError2);
        }

        [Fact]
        public void Equate_Two_Instances_Having_Same_Values()
        {
            var name1 = "jane's";
            var name2 = "june's";

            var organizationNameOrError1 = OrganizationName.Create(name1);
            var organizationNameOrError2 = OrganizationName.Create(name2);

            organizationNameOrError1.Should().NotBeEquivalentTo(organizationNameOrError2);
        }

        [Fact]
        public void Not_Equate_Two_Instances_Having_Differing_Values()
        {
            var name = "jane's";
            Result<OrganizationName> organizationNameOrError = OrganizationName.Create(name);
            organizationNameOrError.Value.Name.Should().Be(name);
            name = "June's";

            string newOrganizationName = OrganizationName.NewOrganizationName(name).Name;

            newOrganizationName.Should().Be(name);
        }

    }
}