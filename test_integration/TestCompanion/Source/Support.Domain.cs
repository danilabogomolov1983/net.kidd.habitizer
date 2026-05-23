using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.TestCompanion.Source;

public static class Support
{
    public static class Domain
    {
        public static SourceName NewSourceName()
        {
            var faker = Wst.Tools.PosiBridge.TestCompanion.Support.NewFaker();
            return new SourceName(faker.Company.CompanyName() + faker.Random.Uuid());
        }

        public static Wst.Tools.PosiBridge.Domain.Source.Source NewSource() =>
            new(SourceId.New(), NewSourceName());
    }
}
