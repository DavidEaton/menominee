﻿using CustomerVehicleManagement.Api.Organizations;
using CustomerVehicleManagement.Api.Persons;
using CustomerVehicleManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerVehicleManagement.Api.Utilities;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using AutoMapper;
using SharedKernel.ValueObjects;
using CustomerVehicleManagement.Shared.Models;

namespace CustomerVehicleManagement.Api.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IPersonRepository personRepository;
        private readonly IOrganizationRepository organizationRepository;
        private readonly IMapper mapper;

        public CustomersController(ICustomerRepository customerRepository,
                                   IPersonRepository personRepository,
                                   IOrganizationRepository organizationRepository,
                                   IMapper mapper)
        {
            this.customerRepository = customerRepository ??
                throw new ArgumentNullException(nameof(customerRepository));
            this.personRepository = personRepository ??
                throw new ArgumentNullException(nameof(personRepository));
            this.organizationRepository = organizationRepository ??
                throw new ArgumentNullException(nameof(organizationRepository));
            this.mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/customers/list
        [Route("list")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerInListDto>>> GetCustomersListAsync()
        {
            var customers = await customerRepository.GetCustomersInListAsync();

            if (customers == null)
                return NotFound();

            return Ok(customers);
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerReadDto>>> GetCustomersAsync()
        {
            var customers = await customerRepository.GetCustomersAsync();

            if (customers == null)
                return NotFound();

            return Ok(customers);
        }

        // GET: api/Customer/1
        [HttpGet("{id:int}", Name = "GetCustomerAsync")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerAsync(int id)
        {
            CustomerReadDto customer = await customerRepository.GetCustomerAsync(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // PUT: api/Customer/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto)
        {
            CustomerReadDto customerFromRepository = await customerRepository.GetCustomerAsync(id);

            if (customerFromRepository == null || customerFromRepository?.EntityType == null)
                return NotFound($"Could not find Customer in the database to update.");

            if (customerFromRepository.EntityType == EntityType.Organization)
            {
                Organization organizationFromRepository = await organizationRepository.GetOrganizationEntityAsync(customerFromRepository.EntityId);

                var organizationNameOrError = OrganizationName.Create(customerUpdateDto.OrganizationUpdateDto.Name);
                if (organizationNameOrError.IsFailure)
                    return BadRequest(organizationNameOrError.Error);

                organizationFromRepository.SetName(organizationNameOrError.Value);
                organizationFromRepository.SetAddress(customerUpdateDto.OrganizationUpdateDto.Address);
                organizationFromRepository.SetNotes(customerUpdateDto.OrganizationUpdateDto.Notes);
                DtoHelpers.PersonUpdateDtoToPerson(customerUpdateDto.OrganizationUpdateDto.Contact, organizationFromRepository.Contact);
                organizationFromRepository.SetPhones(DtoHelpers.PhonesUpdateDtoToPhones(customerUpdateDto.OrganizationUpdateDto.Phones));
                organizationFromRepository.SetEmails(DtoHelpers.EmailsUpdateDtoToEmails(customerUpdateDto.OrganizationUpdateDto.Emails));

                organizationFromRepository.SetTrackingState(TrackingState.Modified);
                customerRepository.FixTrackingState();
            }

            if (customerFromRepository.EntityType == EntityType.Person)
            {
                Person personFromRepository = await personRepository.GetPersonEntityAsync(customerFromRepository.EntityId);
                DtoHelpers.PersonUpdateDtoToPerson(customerUpdateDto.PersonUpdateDto, personFromRepository);

                personFromRepository.SetTrackingState(TrackingState.Modified);
                customerRepository.FixTrackingState();
            }

            if (await customerRepository.SaveChangesAsync())
                return NoContent();

            return BadRequest($"Failed to update .");
        }

        // POST: api/Customer/
        [HttpPost]
        public async Task<ActionResult<CustomerReadDto>> CreateCustomerAsync(CustomerCreateDto customerCreateDto)
        {
            Customer customer = null;

            if (customerCreateDto.PersonCreateDto != null)
            {
                var person = mapper.Map<Person>(customerCreateDto.PersonCreateDto);

                if (person != null)
                {
                    await personRepository.AddPersonAsync(person);

                    if (await personRepository.SaveChangesAsync())
                    {
                        customer = new(person);

                        if (customer != null)
                            await customerRepository.AddCustomerAsync(customer);

                        if (!await customerRepository.SaveChangesAsync())
                            return BadRequest($"Failed to add {customerCreateDto}.");
                    }
                }
            }

            if (customerCreateDto.OrganizationCreateDto != null)
            {
                var organizationNameOrError = OrganizationName.Create(customerCreateDto.OrganizationCreateDto.Name);
                if (organizationNameOrError.IsFailure)
                    return BadRequest(organizationNameOrError.Error);

                var organization = new Organization(organizationNameOrError.Value);

                organization.SetNotes(customerCreateDto.OrganizationCreateDto.Notes);
                organization.SetAddress(customerCreateDto.OrganizationCreateDto.Address);
                organization.SetContact(mapper.Map<Person>(customerCreateDto.OrganizationCreateDto.Contact));

                foreach (var phoneCreateDto in customerCreateDto.OrganizationCreateDto.Phones)
                    organization.AddPhone(new Phone(phoneCreateDto.Number, phoneCreateDto.PhoneType, phoneCreateDto.IsPrimary));

                foreach (var emailCreateDto in customerCreateDto.OrganizationCreateDto.Emails)
                    organization.AddEmail(new Email(emailCreateDto.Address, emailCreateDto.IsPrimary));

                if (organization != null)
                {
                    await organizationRepository.AddOrganizationAsync(organization);

                    if (await organizationRepository.SaveChangesAsync())
                    {
                        customer = new(organization);

                        if (customer != null)
                            await customerRepository.AddCustomerAsync(customer);

                        if (!await customerRepository.SaveChangesAsync())
                            return BadRequest($"Failed to add {customerCreateDto}.");
                    }
                }
            }

            if (customer != null)
            {
                CustomerReadDto customerFromRepository = await customerRepository.GetCustomerAsync(customer.Id);

                return CreatedAtRoute("GetCustomerAsync",
                    new { id = customerFromRepository.Id },
                    customerFromRepository);
            }

            return BadRequest($"Failed to add {customerCreateDto}.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            var customerFromRepository = await customerRepository.GetCustomerAsync(id);
            if (customerFromRepository == null)
                return NotFound($"Could not find Customer in the database to delete with Id: {id}.");

            await customerRepository.DeleteCustomerAsync(id);
            if (await customerRepository.SaveChangesAsync())
                return NoContent();

            return BadRequest($"Failed to delete Customer with Id: {id}.");
        }
    }

}
