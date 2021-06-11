﻿using CustomerVehicleManagement.Api.Customers;
using CustomerVehicleManagement.Api.Persons;
using CustomerVehicleManagement.Api.Phones;
using CustomerVehicleManagement.Domain.Entities;
using SharedKernel.Enums;
using System.Collections.Generic;

namespace CustomerVehicleManagement.Api.Data.Dtos
{
    /// <summary>
    /// The static DTO Extensions class.
    /// Contains static extension methods for converting domain classes to data transfer objects (DTOs).
    /// </summary>
    public static class DtoExtensions
    {
        /// <summary>
        /// Converts a domain Customer to a CustomerReadDto and returns it.
        /// </summary>
        /// <returns>
        /// A CustomerReadDto object.
        /// </returns>
        /// <param name="customer">A domain Customer object</param>
        public static CustomerReadDto ToReadDto(this Customer customer)
        {
            if (customer != null)
            {
                if (customer.EntityType == EntityType.Organization)
                {
                    if (customer.Entity is not Organization organization)
                        return null;

                    return new CustomerReadDto
                    {
                        Id = customer.Id,
                        EntityType = EntityType.Organization,
                        Name = organization.Name.Name,
                        Address = organization.Address,
                        CustomerType = customer.CustomerType,
                        Note = organization.Notes,
                        Contact = (organization.Contact != null)
                        ? new PersonReadDto()
                        {
                            Id = organization.Contact.Id,
                            Address = organization.Contact.Address,
                            Gender = organization.Contact.Gender,
                            Name = organization.Contact.Name.LastFirstMiddle,
                            Phones = (IReadOnlyList<PhoneReadDto>)MapPhones(organization.Contact.Phones)
                        }
                        : null
                    };
                }

                if (customer.EntityType == EntityType.Person)
                {
                    if (customer.Entity is not Person person)
                        return null;

                    return new CustomerReadDto
                    {
                        Id = customer.Id,
                        Name = person.Name.LastFirstMiddle,
                        Address = person.Address,
                        Phones = MapPhones(person.Phones)
                    };
                }
            }

            return null;
        }

        private static IList<PhoneReadDto> MapPhones(IList<Phone> phones)
        {
            IList<PhoneReadDto> phonesDto = new List<PhoneReadDto>();

            foreach (var phone in phones)
            {
                PhoneReadDto phoneReadDto = new PhoneReadDto()
                {
                    Number = phone.Number,
                    PhoneType = phone.PhoneType.ToString(),
                    IsPrimary = phone.IsPrimary
                };

                phonesDto.Add(phoneReadDto);
            }

            return phonesDto;
        }
    }
}
