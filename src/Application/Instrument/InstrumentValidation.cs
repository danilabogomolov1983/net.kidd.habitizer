using LanguageExt;
using Wst.Tools.PosiBridge.Shared.Kernel.Errors;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Instrument;

public static class InstrumentValidation
{
    extension(Domain.Instrument.Instrument that)
    {
        public Fin<Domain.Instrument.Instrument> Validate() =>
            that.ToFin(ValidationErrors.ValidationError("PosiBridge"));
    }
}





