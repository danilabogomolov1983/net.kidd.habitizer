using Net.Kidd.Habitizer.Domain.Instrument;

namespace Net.Kidd.Habitizer.TestCompanion.Instrument;

public static class Support
{
    public static class Domain
    {
        public static Net.Kidd.Habitizer.Domain.Instrument.Instrument NewInstrument() =>
            new(InstrumentId.New(), NewIsin());
    }
}
