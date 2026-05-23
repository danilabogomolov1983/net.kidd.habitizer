using Net.Kidd.Habitizer.Domain.Position;

namespace Net.Kidd.Habitizer.Domain.Test.Position;

public class ExtensionsTests
{
    [Fact]
    public void WithAccount()
    {
        var position = NewPosition();
        var account = NewAccount();

        var actual = position.WithAccount(account);

        Assert.Equal(account, actual.Account);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithInstrument()
    {
        var position = NewPosition();
        var instrument = NewInstrument();

        var actual = position.WithInstrument(instrument);

        Assert.Equal(instrument, actual.Instrument);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithNetSize()
    {
        var position = NewPosition();
        var netSize = TestCompanion.Support.NewDecimal();

        var actual = position.WithNetSize(netSize);

        Assert.Equal(netSize, actual.NetSize);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithNetValue()
    {
        var position = NewPosition();
        var netValue = TestCompanion.Support.NewDecimal();

        var actual = position.WithNetValue(netValue);

        Assert.Equal(netValue, actual.NetValue);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithUnrealisedAverageCost()
    {
        var position = NewPosition();
        var unrealisedAverageCost = TestCompanion.Support.NewDecimal();

        var actual = position.WithUnrealisedAverageCost(unrealisedAverageCost);

        Assert.Equal(unrealisedAverageCost, actual.UnrealisedAverageCost);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithUnrealisedProfit()
    {
        var position = NewPosition();
        var unrealisedProfit = TestCompanion.Support.NewDecimal();

        var actual = position.WithUnrealisedProfit(unrealisedProfit);

        Assert.Equal(unrealisedProfit, actual.UnrealisedProfit);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithUnrealisedProfitPercent()
    {
        var position = NewPosition();
        var unrealisedProfitPercent = TestCompanion.Support.NewDecimal();

        var actual = position.WithUnrealisedProfitPercent(unrealisedProfitPercent);

        Assert.Equal(unrealisedProfitPercent, actual.UnrealisedProfitPercent);
        Assert.NotSame(position, actual);
    }

    [Fact]
    public void WithReferencePrice()
    {
        var position = NewPosition();
        var referencePrice = position.ReferencePrice!;

        var actual = position.WithReferencePrice(referencePrice);

        Assert.Equal(referencePrice, actual.ReferencePrice);
        Assert.NotSame(position, actual);
    }
}
