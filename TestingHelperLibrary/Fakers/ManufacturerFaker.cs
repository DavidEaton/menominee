﻿using Bogus;
using Menominee.Domain.Entities.Inventory;

namespace TestingHelperLibrary.Fakers
{
    public class ManufacturerFaker : Faker<Manufacturer>
    {
        public ManufacturerFaker(bool generateId)
        {
            RuleFor(entity => entity.Id, faker => generateId ? faker.Random.Long(1, 10000) : 0);

            CustomInstantiator(faker =>
            {
                var name = faker.Company.CompanyName();
                var prefix = faker.Random.AlphaNumeric(3).ToUpper();
                var code = name[..3].ToUpper();

                var result = Manufacturer.Create(name, prefix, code);

                return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.Error);
            });
        }
    }
}
