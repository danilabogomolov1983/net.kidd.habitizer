namespace Wst.Tools.PosiBridge.TickTs.Snapshot.ReferencePrice;

public sealed record Response(
    decimal? Price,
    string? CurrencyId,
    string? ExchangeId,
    decimal? CurrencySpot);
