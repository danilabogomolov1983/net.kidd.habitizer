using LanguageExt;
using Net.Kidd.Habitizer.Shared.Kernel.Errors;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Account;

public static class AccountValidation
{
    extension(Domain.Account.Account that)
    {
        public Fin<Domain.Account.Account> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





