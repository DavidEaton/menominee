﻿using Bogus;
using Menominee.Domain.Entities.RepairOrders;

namespace TestingHelperLibrary.Fakers
{
    public class RepairOrderStatusFaker : Faker<RepairOrderStatus>
    {
        public RepairOrderStatusFaker(bool generateId = false, long id = 0)
        {
            if (generateId)
                RuleFor(entity => entity.Id, faker => generateId ? faker.Random.Long(1, 10000) : 0);

            if (id > 0)
                RuleFor(entity => entity.Id, faker => id > 0 ? id : 0);

            CustomInstantiator(faker =>
            {
                var status = faker.PickRandom<Status>();
                var sentence = faker.Lorem.Sentence();
                var description = sentence.Substring(0, Math.Min(50, sentence.Length));

                var result = RepairOrderStatus.Create(status, description);

                return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.Error);
            });
        }
    }
}