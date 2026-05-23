using LanguageExt;
using Wst.Tools.PosiBridge.Shared.Kernel.Errors;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source;

public static class SourceValidation
{
    extension(Domain.Source.Source that)
    {
        public Fin<Domain.Source.Source> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





