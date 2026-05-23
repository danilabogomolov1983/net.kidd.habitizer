using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Instrument;

namespace Net.Kidd.Habitizer.Domain.Position;

public static class Extensions
{
    extension(Position position)
    {
        public bool IsValid() => !(position.Account.IsEmpty() && position.Instrument.IsEmpty());
        public Position WithAccount(Domain.Account.Account account) =>
            position with { Account = account };

        public Position WithInstrument(Domain.Instrument.Instrument instrument) =>
            position with { Instrument = instrument };

        public Position WithNetSize(decimal? netSize) =>
            position with { NetSize = netSize };

        public Position WithNetValue(decimal? netValue) =>
            position with { NetValue = netValue };

        public Position WithUnrealisedAverageCost(decimal? unrealisedAverageCost) =>
            position with { UnrealisedAverageCost = unrealisedAverageCost };

        public Position WithUnrealisedProfit(decimal? unrealisedProfit) =>
            position with { UnrealisedProfit = unrealisedProfit };

        public Position WithUnrealisedProfitPercent(decimal? unrealisedProfitPercent) =>
            position with { UnrealisedProfitPercent = unrealisedProfitPercent };

        public Position WithReferencePrice(ReferencePrice.ReferencePrice? referencePrice) =>
            position with { ReferencePrice = referencePrice };
    }
}
