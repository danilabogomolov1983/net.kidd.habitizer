using FluentValidation;

namespace Wst.Tools.PosiBridge.Application.Snapshot.Configuration;

public class SyncSettingsValidator : AbstractValidator<SyncSettings>
{
    public SyncSettingsValidator()
    {
        RuleFor(x => x.Accounts).NotEmpty();
    }
}
