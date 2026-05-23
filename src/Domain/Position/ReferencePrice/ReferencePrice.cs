namespace Net.Kidd.Habitizer.Domain.Position.ReferencePrice;

public sealed record ReferencePrice(
    decimal? Price,
    string? Currency,
    string? Exchange,
    decimal? CurrencySpot)
{
    public static ReferencePrice Empty() => new(
        null,
        null,
        null,
        null);
}
