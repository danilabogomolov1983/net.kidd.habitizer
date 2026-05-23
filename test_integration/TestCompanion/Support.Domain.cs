using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.TestCompanion;

public static partial class Support
{
    public static class Domain
    {
        public static Isin NewIsin() => new (NewFaker().Random.Replace("??##########"));
    }
}
