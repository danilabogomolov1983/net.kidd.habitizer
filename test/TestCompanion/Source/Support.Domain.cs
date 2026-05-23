using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.TestCompanion.Source;

public static class Support
{
    public static class Domain
    {
        public static SourceName NewSourceName() => new(Net.Kidd.Habitizer.TestCompanion.Support.NewFaker().Company.CompanyName() + Guid.NewGuid());

        public static Net.Kidd.Habitizer.Domain.Source.Source NewSource() => new (SourceId.New(), NewSourceName());

    }
}
