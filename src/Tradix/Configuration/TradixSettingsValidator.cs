using FluentValidation;

namespace Net.Kidd.Habitizer.Tradix.Configuration;

public class TradixSettingsValidator : AbstractValidator<TradixSettings>
{
    public TradixSettingsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
        
    }
}
