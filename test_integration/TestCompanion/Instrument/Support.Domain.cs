using Wst.Tools.PosiBridge.Domain.Instrument;

namespace Wst.Tools.PosiBridge.TestCompanion.Instrument;

public static class Support
{
    public static class Domain
    {
        public static Wst.Tools.PosiBridge.Domain.Instrument.Instrument NewInstrument() =>
            new(InstrumentId.New(), NewIsin());
    }
}
