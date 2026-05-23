using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Instrument;

public static class Extensions
{
    extension(Instrument instrument)
    {
        public Instrument WithIsin(Isin isin) =>
            instrument with { Isin = isin };
        
        public bool IsEmpty() => instrument == Instrument.Empty();
    }
}
