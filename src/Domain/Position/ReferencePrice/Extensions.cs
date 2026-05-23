namespace Wst.Tools.PosiBridge.Domain.Position.ReferencePrice;

public static class Extensions
{
    extension(ReferencePrice referencePrice)
    {
        public ReferencePrice WithPrice(decimal? price) =>
            referencePrice with { Price = price };

        public ReferencePrice WithCurrency(string? currency) =>
            referencePrice with { Currency = currency };

        public ReferencePrice WithExchange(string? exchange) =>
            referencePrice with { Exchange = exchange };

        public ReferencePrice WithCurrencySpot(decimal? currencySpot) =>
            referencePrice with { CurrencySpot = currencySpot };
    }
}
