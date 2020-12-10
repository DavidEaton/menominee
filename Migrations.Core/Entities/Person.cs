﻿using Migrations.Core.Enums;
using Migrations.Core.ValueObjects;
using SharedKernel.Enums;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migrations.Core.Entities
{
    public class Person : IEntity
    {
        // EF reuires an empty constructor
        protected Person()
        {
        }

        public Person(string lastName, string firstName, string middleName = null, DriversLicence driversLicence = null)
        {
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First and Last Names cannot be empty");
            }

            LastName = lastName;
            FirstName = firstName;
            MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName;
            Phones = new List<Phone>();
            DriversLicence = driversLicence;
            // Keep EF informed of object state in disconnected api
            TrackingState = TrackingState.Added;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(1)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(1)]
        public string FirstName { get; set; }

        [MaxLength(255)]
        [MinLength(1)]
        public string MiddleName { get; set; }
        public DateTime? Birthday { get; set; }

        [StringLength(1)]
        public Gender Gender { get; set; }
        public DriversLicence DriversLicence { get; set; }
        public string NameLastFirst
        {
            get => string.IsNullOrWhiteSpace(MiddleName) ? $"{LastName}, {FirstName}" : $"{LastName}, {FirstName} {MiddleName}";
        }
        public string NameFirstLast
        {
            get => string.IsNullOrWhiteSpace(MiddleName) ? $"{FirstName} {LastName}" : $"{FirstName} {MiddleName} {LastName}";
        }
        public Address Address { get; set; }
        public ICollection<Phone> Phones { get; set; }

        // EF State management for disconnected data
        public void UpdateState(TrackingState state)
        {
            TrackingState = state;
        }

        [NotMapped]
        public TrackingState TrackingState { get; private set; }

    }
}
