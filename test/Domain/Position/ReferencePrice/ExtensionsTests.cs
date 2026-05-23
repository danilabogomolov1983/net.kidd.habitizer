using Wst.Tools.PosiBridge.Domain.Position.ReferencePrice;

namespace Wst.Tools.PosiBridge.Domain.Test.Position.ReferencePrice;

public class ExtensionsTests
{
    [Fact]
    public void WithPrice()
    {
        var referencePrice = NewPosition().ReferencePrice!;
        var price = TestCompanion.Support.NewDecimal();

        var actual = referencePrice.WithPrice(price);

        Assert.Equal(price, actual.Price);
        Assert.NotSame(referencePrice, actual);
    }

    [Fact]
    public void WithCurrency()
    {
        var referencePrice = NewPosition().ReferencePrice!;
        var currency = NewCurrency();

        var actual = referencePrice.WithCurrency(currency);

        Assert.Equal(currency, actual.Currency);
        Assert.NotSame(referencePrice, actual);
    }

    [Fact]
    public void WithExchange()
    {
        var referencePrice = NewPosition().ReferencePrice!;
        var exchange = NewExchange();

        var actual = referencePrice.WithExchange(exchange);

        Assert.Equal(exchange, actual.Exchange);
        Assert.NotSame(referencePrice, actual);
    }

    [Fact]
    public void WithCurrencySpot()
    {
        var referencePrice = NewPosition().ReferencePrice!;
        var currencySpot = TestCompanion.Support.NewDecimal();

        var actual = referencePrice.WithCurrencySpot(currencySpot);

        Assert.Equal(currencySpot, actual.CurrencySpot);
        Assert.NotSame(referencePrice, actual);
    }
}
