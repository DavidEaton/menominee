﻿namespace Menominee.Shared.Models.Persons.PersonNames
{
    public class PersonNameToRead
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; } = string.Empty;

        public string LastFirstMiddle
        {
            get => string.IsNullOrWhiteSpace(MiddleName) ? $"{LastName}, {FirstName}" : $"{LastName}, {FirstName} {MiddleName}";
        }

        public string LastFirstMiddleInitial
        {
            get => string.IsNullOrWhiteSpace(MiddleName) ? $"{LastName}, {FirstName}" : $"{LastName}, {FirstName} {MiddleName[0]}.";
        }
        public string FirstMiddleLast
        {
            get => string.IsNullOrWhiteSpace(MiddleName) ? $"{FirstName} {LastName}" : $"{FirstName} {MiddleName} {LastName}";
        }

        public override string ToString()
        {
            return LastFirstMiddleInitial;
        }

        public static implicit operator string(PersonNameToRead personNameToRead)
        {
            return personNameToRead?.ToString() ?? string.Empty;
        }
    }
}
