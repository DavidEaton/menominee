﻿using CustomerVehicleManagement.Api.Persons;
using CustomerVehicleManagement.Domain.Entities;
using CustomerVehicleManagement.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedKernel.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CustomerVehicleManagement.Api.IntegrationTests.Controllers
{
    public class PersonsControllerShould
    {
        private readonly PersonsController controller;
        private readonly Mock<IPersonRepository> moqRepository;

        public PersonsControllerShould()
        {
            moqRepository = new Mock<IPersonRepository>();
            controller = new PersonsController(moqRepository.Object);
        }

        #region ********************************Get***********************************

        [Fact]
        public async Task Return_ActionResult_Of_PersonReadDto_On_GetPersonAsync()
        {
            var result = await controller.GetPersonAsync(1);

            result.Result.Should().BeOfType<NotFoundResult>();
            result.Should().BeOfType<ActionResult<PersonToRead>>();
        }

        [Fact]
        public async Task Return_NotFoundResult_On_GetPersonAsyncWithInvalidId()
        {
            var result = await controller.GetPersonAsync(0);

            result.Should().BeOfType<ActionResult<PersonToRead>>();
        }

        [Fact]
        public async Task Return_ActionResult_Of_IEnumerable_Of_PersonReadDto_On_GetPersonsAsync()
        {
            var result = await controller.GetPersonsAsync();

            result.Should().BeOfType<ActionResult<IReadOnlyList<PersonToRead>>>();
        }

        [Fact]
        public async Task Return_ActionResult_Of_IEnumerable_Of_PersonReadDto_On_GetPersonsListAsync()
        {
            var result = await controller.GetPersonsListAsync();

            result.Should().BeOfType<ActionResult<IReadOnlyList<PersonToReadInList>>>();
        }

        #endregion Get

        #region ********************************Post**********************************

        [Fact]
        public async Task Return_ActionResult_Of_PersonReadDto_On_CreatePersonAsync()
        {
            var person = new PersonToAdd { Name = new PersonNameToAdd { LastName = "Doe", FirstName = "Jane" }, Gender = Gender.Female };

            var result = await controller.CreatePersonAsync(person);

            result.Should().BeOfType<ActionResult<PersonToRead>>();
        }

        [Fact]
        public async Task Return_BadRequestObjectResult_On_CreatePersonAsync_When_ModelState_Invalid()
        {
            controller.ModelState.AddModelError("x", "Test Error Message");
            var person = new PersonToAdd { Name = new PersonNameToAdd { LastName = "Doe", FirstName = "Jane" }, Gender = Gender.Female };

            var result = await controller.CreatePersonAsync(person);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
            moqRepository.Verify(repo => repo.AddPersonAsync(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public async Task Not_Save_On_CreatePersonAsync_When_ModelState_Invalid()
        {
            controller.ModelState.AddModelError("x", "Test Error Message");
            var person = new PersonToAdd { Name = new PersonNameToAdd { LastName = "Doe", FirstName = "Jane" }, Gender = Gender.Female };

            var result = await controller.CreatePersonAsync(person);

            moqRepository.Verify(repo => repo.AddPersonAsync(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public async Task Save_On_CreatePersonAsync_When_ModelState_Valid()
        {
            Person savedPerson = null;

            moqRepository.Setup(repo => repo.AddPersonAsync(It.IsAny<Person>()))
                          .Returns(Task.CompletedTask)
                          .Callback<Person>(person => savedPerson = person);

            var person = new PersonToAdd { Name = new PersonNameToAdd { LastName = "Doe", MiddleName = "J.", FirstName = "Jane" }, Gender = Gender.Female };

            var result = await controller.CreatePersonAsync(person);

            moqRepository.Verify(repository => repository.AddPersonAsync(It.IsAny<Person>()), Times.Once);
            person.Name.LastName.Should().Be(savedPerson.Name.LastName);
            person.Name.FirstName.Should().Be(savedPerson.Name.FirstName);
            person.Name.MiddleName.Should().Be(savedPerson.Name.MiddleName);
            person.Gender.Should().Be(savedPerson.Gender);
            person.Birthday.Should().Be(savedPerson.Birthday);
        }

        [Fact]
        public async Task Return_PersonReadDto_On_CreatePersonAsync_When_ModelState_Valid()
        {
            moqRepository.Setup(repository => repository.AddPersonAsync(It.IsAny<Person>()));

            var person = new PersonToAdd { Name = new PersonNameToAdd { LastName = "Doe", FirstName = "Jane" }, Gender = Gender.Female };
            var result = await controller.CreatePersonAsync(person);

            result.Should().BeOfType<ActionResult<PersonToRead>>();
        }

        #endregion Post

        #region ********************************Put***********************************

        [Fact]
        public async Task Return_NotFoundObjectResult_On_UpdatePersonAsync_With_Invalid_Id()
        {
            var invaldId = 0;
            var person = new PersonToEdit
            {
                Gender = Gender.Female
            };

            var result = await controller.UpdatePersonAsync(invaldId, person);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        //[Fact]
        //public async Task Return_NoContent_On_UpdatePersonAsync()
        //{
        //}
        #endregion Put
    }
}
