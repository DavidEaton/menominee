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

namespace CustomerVehicleManagement.Api.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IPersonRepository personRepository;
        private readonly IOrganizationRepository organizationRepository;
        private const int MaxCacheAge = 300;
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

        // GET: api/Customers
        [HttpGet]
        [ResponseCache(Duration = MaxCacheAge)]
        public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetCustomersAsync()
        {
            var results = await customerRepository.GetCustomersAsync();
            return Ok(results);
        }

        // GET: api/Customer/1
        [HttpGet("{id:int}", Name = "GetCustomerAsync")]
        public async Task<ActionResult<CustomerReadDto>> GetCustomerAsync(int id)
        {
            var customer = await customerRepository.GetCustomerAsync(id);

            if (customer == null)
                return NotFound();

            CustomerReadDto customerReadDto = new()
            {
                Id = customer.Id,
                CustomerType = customer.CustomerType,
                EntityType = customer.EntityType
            };

            if (customer.EntityType == EntityType.Organization)
            {
                await customerRepository.GetCustomerOrganizationEntity(customer);
                customerReadDto.Address = ((Organization)customer.Entity).Address;
                customerReadDto.Name = ((Organization)customer.Entity).Name.Name;
                //customerReadDto.Phones = ((Organization)customer.Entity).Phones;
            }

            if (customer.EntityType == EntityType.Person)
            {
                await customerRepository.GetCustomerPersonEntity(customer);
                customerReadDto.Address = ((Person)customer.Entity).Address;
                customerReadDto.Name = ((Person)customer.Entity).Name.LastFirstMiddle;
            }

            return customerReadDto;
        }

        // PUT: api/Customer/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto)
        {
            /* Update Pattern in Controllers:
                1) Get domain entity from repository
                2) Update domain entity with data in data transfer object (DTO)
                3) Set entity's TrackingState to Modified
                4) FixTrackingState: moves entity state tracking back out of
                the object and into the context to track entity state in this
                disconnected applications. In other words, sych the EF Change
                Tracker with our disconnected entity's TrackingState
                5) Save changes
                6) return NoContent()
            */

            //1) Get domain entity from repository
            var fetchedCustomer = await customerRepository.GetCustomerAsync(id);

            if (fetchedCustomer == null || fetchedCustomer?.EntityType == EntityType.Organization)
                return NotFound($"Could not find Customer in the database to update.");

            // 2) Update domain entity with data in data transfer object(DTO)
            if (fetchedCustomer.EntityType == EntityType.Organization)
            {
                //1.a) Get domain Organization from repository
                Organization organizationFromRepository = await organizationRepository.GetOrganizationEntityAsync(fetchedCustomer.EntityId);

                // 2.a) Update domain entity with data in data transfer object(DTO)
                DtoHelpers.ConvertOrganizationUpdateDtoToDomainModel(customerUpdateDto.OrganizationUpdateDto, organizationFromRepository, mapper);

                // Update the objects ObjectState and sych the EF Change Tracker
                // 3) Set entity's TrackingState to Modified
                fetchedCustomer.SetTrackingState(TrackingState.Modified);
                // 4) FixTrackingState: moves entity state tracking into the context
                customerRepository.FixTrackingState();
            }

            if (fetchedCustomer.EntityType == EntityType.Person)
            {
                //1.a) Get domain Person from repository
                Person personFromRepository = await personRepository.GetPersonEntityAsync(fetchedCustomer.EntityId);
                // 2.a) Update domain entity with data in data transfer object(DTO)
                DtoHelpers.ConvertPersonUpdateDtoToDomainModel(customerUpdateDto.PersonUpdateDto, personFromRepository, mapper);

                // Update the objects ObjectState and sych the EF Change Tracker
                // 3) Set entity's TrackingState to Modified
                fetchedCustomer.SetTrackingState(TrackingState.Modified);
                // 4) FixTrackingState: moves entity state tracking into the context
                customerRepository.FixTrackingState();
            }

            //5) Save changes
            if (await customerRepository.SaveChangesAsync())
                // 6) return NoContent()
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
                var person = new Person(customerCreateDto.PersonCreateDto.Name, customerCreateDto.PersonCreateDto.Gender);
                // TODO: Add Birthday, DriversLicense, Address, Phones, Emails
                //...

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
                // TODO: Add Contact, DriversLicense, Address, Phones, Emails
                //...

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
                string name = string.Empty;

                if (customer.EntityType == EntityType.Organization)
                    name = customerCreateDto.OrganizationCreateDto.Name;

                if (customer.EntityType == EntityType.Person)
                    name = customerCreateDto.PersonCreateDto.Name.LastFirstMiddle;

                var customerReadDto = new CustomerReadDto
                {
                    Id = customer.Id,
                    EntityType = customer.EntityType,
                    Name = name
                };

                return CreatedAtRoute("GetCustomerAsync",
                    new { id = customerReadDto.Id },
                    customerReadDto);
            }

            return BadRequest($"Failed to add {customerCreateDto}.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            var fetchedCustomer = await customerRepository.GetCustomerAsync(id);
            if (fetchedCustomer == null)
                return NotFound($"Could not find Customer in the database to delete with Id: {id}.");

            customerRepository.DeleteCustomer(fetchedCustomer);
            if (await customerRepository.SaveChangesAsync())
            {
                return Ok();
            }

            return BadRequest($"Failed to delete Customer with Id: {id}.");
        }
    }

}
