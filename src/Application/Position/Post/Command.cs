using Wst.Tools.PosiBridge.Domain.Position.ReferencePrice;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Position.Post;

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
