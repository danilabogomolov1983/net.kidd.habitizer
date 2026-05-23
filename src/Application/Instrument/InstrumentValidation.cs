using LanguageExt;
using Net.Kidd.Habitizer.Shared.Kernel.Errors;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Instrument;

public static class InstrumentValidation
{
    extension(Domain.Instrument.Instrument that)
    {
        public Fin<Domain.Instrument.Instrument> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





