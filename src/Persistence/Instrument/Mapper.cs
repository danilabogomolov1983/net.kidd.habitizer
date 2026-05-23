using Net.Kidd.Habitizer.Domain.ValueObjects;
using Net.Kidd.Habitizer.Shared.Kernel;
using InstrumentId = Net.Kidd.Habitizer.Domain.Instrument.InstrumentId;

namespace Net.Kidd.Habitizer.Persistence.Instrument;

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
