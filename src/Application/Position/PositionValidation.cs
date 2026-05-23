using LanguageExt;
using Wst.Tools.PosiBridge.Shared.Kernel.Errors;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Position;

public static class PositionValidation
{
    extension(Domain.Position.Position that)
    {
        public Fin<Domain.Position.Position> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





