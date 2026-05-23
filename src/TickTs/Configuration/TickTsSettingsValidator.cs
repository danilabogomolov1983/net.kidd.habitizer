using FluentValidation;

namespace Net.Kidd.Habitizer.TickTs.Configuration;

public class TickTsSettingsValidator : AbstractValidator<TickTsSettings>
{
    public TickTsSettingsValidator()
    {
        RuleFor(x => x.BaseUrl).NotEmpty();
        
        RuleFor(x => x.BaseUrl)
            .Must(baseUrl => Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.BaseUrl))
            .WithMessage($"{nameof(TickTsSettings.BaseUrl)} must be a valid URL");

        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.ResolvedAddress).NotEmpty();
    }
}
