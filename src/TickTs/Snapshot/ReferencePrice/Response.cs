namespace Net.Kidd.Habitizer.TickTs.Snapshot.ReferencePrice;

public sealed record Response(
    decimal? Price,
    string? CurrencyId,
    string? ExchangeId,
    decimal? CurrencySpot);
