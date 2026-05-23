using FluentValidation;

namespace Wst.Tools.PosiBridge.Tradix.Configuration;

public class TradixSettingsValidator : AbstractValidator<TradixSettings>
{
    public TradixSettingsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
        
    }
}
