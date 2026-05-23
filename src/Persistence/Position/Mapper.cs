using Net.Kidd.Habitizer.Domain.Position.ReferencePrice;
using Net.Kidd.Habitizer.Shared.Kernel;

namespace Net.Kidd.Habitizer.Persistence.Position;

public struct Mapper : IMap<Domain.Position.Position, PositionDbo>
{
    public Domain.Position.Position Map(PositionDbo right)
    {
        ArgumentNullException.ThrowIfNull(right.Account);
        ArgumentNullException.ThrowIfNull(right.Account.Source);
        ArgumentNullException.ThrowIfNull(right.Instrument);

        var hasReferencePrice =
            right.ReferencePrice_Price.HasValue ||
            right.ReferencePrice_Currency != null ||
            right.ReferencePrice_Exchange != null ||
            right.ReferencePrice_CurrencySpot.HasValue;

        return new Domain.Position.Position(
            DboMappers.AccountMapper.Map(right.Account),
            DboMappers.InstrumentMapper.Map(right.Instrument),
            right.NetSize,
            right.NetValue,
            right.UnrealisedAverageCost,
            right.UnrealisedProfit,
            right.UnrealisedProfitPercent,
            hasReferencePrice
                ? new ReferencePrice(
                    right.ReferencePrice_Price,
                    right.ReferencePrice_Currency,
                    right.ReferencePrice_Exchange,
                    right.ReferencePrice_CurrencySpot)
                : null);
    }

    public PositionDbo Map(Domain.Position.Position left)
    {
        ArgumentNullException.ThrowIfNull(left.Account);
        ArgumentNullException.ThrowIfNull(left.Instrument);

        return new PositionDbo
        {
            AccountId = left.Account.Id.Value,
            InstrumentId = left.Instrument.Id.Value,
            Account = DboMappers.AccountMapper.Map(left.Account),
            Instrument = DboMappers.InstrumentMapper.Map(left.Instrument),
            NetSize = left.NetSize,
            NetValue = left.NetValue,
            UnrealisedAverageCost = left.UnrealisedAverageCost,
            UnrealisedProfit = left.UnrealisedProfit,
            UnrealisedProfitPercent = left.UnrealisedProfitPercent,
            ReferencePrice_Price = left.ReferencePrice?.Price,
            ReferencePrice_Currency = left.ReferencePrice?.Currency,
            ReferencePrice_Exchange = left.ReferencePrice?.Exchange,
            ReferencePrice_CurrencySpot = left.ReferencePrice?.CurrencySpot
        };
    }
}
