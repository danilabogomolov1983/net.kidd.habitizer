using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Instrument;

public sealed record Instrument(InstrumentId Id, Isin Isin)
{
    public static Instrument Empty() => new(InstrumentId.Empty(), Isin.Empty());
};
