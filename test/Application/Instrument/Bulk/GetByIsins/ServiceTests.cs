using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.TestCompanion;
using GetByIsinsInstrument = Wst.Tools.PosiBridge.Application.Instrument.Bulk.GetByIsins;

namespace Wst.Tools.PosiBridge.Application.Test.Instrument.Bulk.GetByIsins;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly BulkStore _bulkStore = new(fixture.ContextFactory);
    private readonly GetByIsinsInstrument.Service _service = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task GetByIsinsAsync_ReturnsInstrumentsFromBulkStore()
    {
        var instrument1 = NewInstrument();
        var instrument2 = NewInstrument();

        Assert.True((await _bulkStore.InsertAsync([instrument1, instrument2])).IsSucc);

        var actual = await _service.GetByIsinsAsync(new GetByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));
        Assert.True(actual.IsSucc);
        var actualInstruments = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualInstruments.Count);
        Assert.Contains(instrument1, actualInstruments);
        Assert.Contains(instrument2, actualInstruments);
    }

    [Fact]
    public async Task GetByIsinsAsync_InstrumentsDoNotExist()
    {
        var instrument1 = NewInstrument();
        var instrument2 = NewInstrument();

        var actual = await _service.GetByIsinsAsync(new GetByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));
        Assert.True(actual.IsFail);
    }
}
