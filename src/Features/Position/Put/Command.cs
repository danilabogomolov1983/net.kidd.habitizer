using Net.Kidd.Habitizer.Domain.Position.ReferencePrice;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Position.Put;

public sealed record Command(
    SourceName SourceName,
    AccountName AccountName,
    Isin Isin,
    decimal? NetSize,
    decimal? NetValue,
    decimal? UnrealisedAverageCost,
    decimal? UnrealisedProfit,
    decimal? UnrealisedProfitPercent,
    ReferencePrice? ReferencePrice);
