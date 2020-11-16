﻿using EntityApp.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityApp.Domain.Classes
{
    public class Person : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }

        public DriversLicence DriversLicence { get; set; }
        public int? DriversLicenceId { get; set; }
    }
}
