using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Instrument;

public sealed record Instrument(InstrumentId Id, Isin Isin)
{
    public static Instrument Empty() => new(InstrumentId.Empty(), Isin.Empty());
};
