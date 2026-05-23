using LanguageExt;
using Wst.Tools.PosiBridge.Shared.Kernel.Errors;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Account;

public static class AccountValidation
{
    extension(Domain.Account.Account that)
    {
        public Fin<Domain.Account.Account> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





