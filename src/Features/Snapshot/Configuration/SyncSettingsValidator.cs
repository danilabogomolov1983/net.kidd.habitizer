using FluentValidation;

namespace Net.Kidd.Habitizer.Features.Snapshot.Configuration;

public class SyncSettingsValidator : AbstractValidator<SyncSettings>
{
    public SyncSettingsValidator()
    {
        RuleFor(x => x.Accounts).NotEmpty();
    }
}
