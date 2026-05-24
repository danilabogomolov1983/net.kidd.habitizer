using LanguageExt;
using Net.Kidd.Habitizer.Shared.Kernel.Errors;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Source;

public static class SourceValidation
{
    extension(Domain.Source.Source that)
    {
        public Fin<Domain.Source.Source> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("Habitizer"));
    }
}





