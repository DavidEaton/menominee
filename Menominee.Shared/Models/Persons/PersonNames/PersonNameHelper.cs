﻿using Menominee.Domain.ValueObjects;

namespace Menominee.Shared.Models.Persons.PersonNames
{
    public class PersonNameHelper
    {
        public static PersonNameToRead ConvertToReadDto(PersonName name)
        {
            return name is not null
                ? new()
                {
                    FirstName = name.FirstName,
                    MiddleName = name.MiddleName,
                    LastName = name.LastName
                }
                : new();
        }

        public static PersonNameToWrite ConvertToWriteDto(PersonName name)
        {
            return name is null
                ? new()
                : new()
                {
                    FirstName = name.FirstName,
                    MiddleName = name.MiddleName,
                    LastName = name.LastName
                };
        }

        internal static PersonNameToWrite ConvertReadToWriteDto(PersonNameToRead name)
        {
            return name is null
                ? new()
                : new()
                {
                    FirstName = name.FirstName,
                    MiddleName = name.MiddleName,
                    LastName = name.LastName
                };
        }

        internal static PersonNameToRead ConvertWriteToReadDto(PersonNameToWrite name)
        {
            return name is null
                ? new()
                : new()
                {
                    FirstName = name.FirstName,
                    MiddleName = name.MiddleName,
                    LastName = name.LastName
                };
        }
    }
}
