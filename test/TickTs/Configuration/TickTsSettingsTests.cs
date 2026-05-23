using FluentValidation.TestHelper;
using Net.Kidd.Habitizer.TickTs.Configuration;

namespace Net.Kidd.Habitizer.TickTs.Test.Configuration;

public class TickTsSettingsTests
{
    [Fact]
    public void TickTsSettings_Validate_Success()
    {
        var faker = NewFaker();
        var validator = new TickTsSettingsValidator();

        var settings = new TickTsSettings
        {
            BaseUrl = faker.Internet.Url(),
            Token = faker.Random.String2(10),
            ResolvedAddress = faker.Internet.IpAddress().ToString()
        };

        var actual = validator.TestValidate(settings);

        Assert.NotNull(actual);
        Assert.True(actual.IsValid);
    }
    
    [Fact]
    public void TickTsSettings_Validate_Errors()
    {
        var faker = NewFaker();
        var validator = new TickTsSettingsValidator();

        var invalidSettings = new TickTsSettings();

        var actual = validator.TestValidate(invalidSettings);

        Assert.NotNull(actual);
        Assert.False(actual.IsValid);
        
        actual.ShouldHaveValidationErrorFor(x => x.BaseUrl);
        actual.ShouldHaveValidationErrorFor(x => x.Token);
        actual.ShouldHaveValidationErrorFor(x => x.ResolvedAddress);
    }
}
