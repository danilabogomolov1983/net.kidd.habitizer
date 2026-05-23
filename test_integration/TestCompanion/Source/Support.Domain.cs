using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.TestCompanion.Source;

public static class Support
{
    public static class Domain
    {
        public static SourceName NewSourceName()
        {
            var faker = Net.Kidd.Habitizer.TestCompanion.Support.NewFaker();
            return new SourceName(faker.Company.CompanyName() + faker.Random.Uuid());
        }

        public static Net.Kidd.Habitizer.Domain.Source.Source NewSource() =>
            new(SourceId.New(), NewSourceName());
    }
}
