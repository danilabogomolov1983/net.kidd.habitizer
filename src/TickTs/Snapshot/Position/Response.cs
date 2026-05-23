namespace Wst.Tools.PosiBridge.TickTs.Snapshot.Position;

public sealed record Response(
    string Id,
    string InstrumentId,
    decimal? NetSize,
    decimal? NetValue,
    decimal? UnrealisedAverageCost,
    decimal? UnrealisedProfit,
    decimal? UnrealisedProfitPercent,
    ReferencePrice.Response? ReferencePrice = null);
