using Net.Kidd.Habitizer.Domain.ValueObjects;
using Net.Kidd.Habitizer.Shared.Kernel;

namespace Net.Kidd.Habitizer.TickTs.Snapshot.Position;

public struct Mapper : IOnewayMap<Domain.Position.Position, (Domain.Account.Account, Response)>
{
    public Domain.Position.Position Map((Domain.Account.Account, Response) left)
    {
        var (account, response) = left;
        var instrument = new Domain.Instrument.Instrument(Domain.Instrument.InstrumentId.Empty(), new Isin(response.InstrumentId));
        return new Domain.Position.Position(
            account,
            instrument,
            response.NetSize,
            response.NetValue,
            response.UnrealisedAverageCost,
            response.UnrealisedProfit,
            response.UnrealisedProfitPercent,
            response.ReferencePrice != null ? TickTsMappers.SnapshotReferencePriceMapper.Map(response.ReferencePrice) : null);
    }
}
