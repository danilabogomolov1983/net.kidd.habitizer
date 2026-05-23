using LanguageExt;
using Net.Kidd.Habitizer.Shared.Kernel.Errors;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Position;

public static class PositionValidation
{
    extension(Domain.Position.Position that)
    {
        public Fin<Domain.Position.Position> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





