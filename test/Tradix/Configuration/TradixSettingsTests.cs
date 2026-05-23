using Wst.Tools.PosiBridge.Tradix.Configuration;

namespace Wst.Tools.PosiBridge.Tradix.Test.Configuration;

public class TradixSettingsTests
{
    [Fact]
    public void TradixSettings_Validate_Success()
    {
        var validator = new TradixSettingsValidator();

        var settings = new TradixSettings
        {
            ConnectionString = "Server=localhost;Database=tradix;Trusted_Connection=True;TrustServerCertificate=True;"
        };

        var actual = validator.Validate(settings);

        Assert.True(actual.IsValid);
        Assert.Empty(actual.Errors);
    }

    [Fact]
    public void TradixSettings_Validate_Errors()
    {
        var validator = new TradixSettingsValidator();

        var actual = validator.Validate(new TradixSettings());

        Assert.False(actual.IsValid);
        Assert.Contains(actual.Errors, error => error.PropertyName == nameof(TradixSettings.ConnectionString));
    }
}
