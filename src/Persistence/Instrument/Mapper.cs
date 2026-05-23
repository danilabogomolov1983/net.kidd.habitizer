using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel;
using InstrumentId = Wst.Tools.PosiBridge.Domain.Instrument.InstrumentId;

namespace Wst.Tools.PosiBridge.Persistence.Instrument;

public struct Mapper : IMap<Domain.Instrument.Instrument, InstrumentDbo>
{
    public Domain.Instrument.Instrument Map(InstrumentDbo right)
    {
        return new Domain.Instrument.Instrument(new InstrumentId(right.Id), new Isin(right.Isin));
    }

    public InstrumentDbo Map(Domain.Instrument.Instrument left)
    {
        return new InstrumentDbo
        {
            Id = left.Id.Value,
            Isin = left.Isin.Value
        };
    }
}
