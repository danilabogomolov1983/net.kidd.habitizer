using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.TestCompanion;

public static partial class Support
{
    public static class Domain
    {
        public static Isin NewIsin() => new (NewFaker().Random.Replace("??##########"));
    }
}
