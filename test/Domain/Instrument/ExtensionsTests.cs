using Wst.Tools.PosiBridge.Domain.Instrument;

namespace Wst.Tools.PosiBridge.Domain.Test.Instrument;

public class ExtensionsTests
{
    [Fact]
    public void WithIsin()
    {
        var instrument = NewInstrument();
        var isin = NewIsin();

        var actual = instrument.WithIsin(isin);

        Assert.Equal(isin, actual.Isin);
        Assert.NotSame(instrument, actual);
    }
}
