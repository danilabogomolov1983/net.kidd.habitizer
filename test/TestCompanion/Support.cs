using LanguageExt.Common;

namespace Wst.Tools.PosiBridge.TestCompanion;

public static partial class Support
{
    public static decimal NewDecimal(int scale = 2) => Math.Round(NewFaker().Random.Decimal(1, 1000), scale);

    public static Bogus.Faker NewFaker() => new();

    public static Action<Error> AssertError(Error expectedError) => actualError =>
    {
        Assert.NotNull(actualError);
        Assert.Equal(expectedError.Message, actualError.Message);
    };
}

