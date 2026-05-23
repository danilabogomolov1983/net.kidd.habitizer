using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.TestCompanion;
using DeleteByIsinsInstrument = Wst.Tools.PosiBridge.Application.Instrument.Bulk.DeleteByIsins;
using GetByIsinsInstrument = Wst.Tools.PosiBridge.Application.Instrument.Bulk.GetByIsins;
using InstrumentAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.Test.Instrument.Bulk.DeleteByIsins;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly InstrumentAddMissing.Service _addMissingService = new(new BulkStore(fixture.ContextFactory));
    private readonly DeleteByIsinsInstrument.Service _service = new(new GetByIsinsInstrument.Service(new BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly GetByIsinsInstrument.Service _getByIsinsService = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task DeleteAsync_DeletesIsinsUsingBulkStore()
    {
        var instrument1 = NewInstrument();
        var instrument2 = NewInstrument();

        Assert.True((await _addMissingService.AddMissingAsync(new InstrumentAddMissing.Command([instrument1, instrument2]))).IsSucc);

        var result = await _service.DeleteAsync(new DeleteByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));
        Assert.True(result.IsSucc);

        var actual = await _getByIsinsService.GetByIsinsAsync(new GetByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteAsync_InstrumentsDoNotExist()
    {
        var instrument1 = NewInstrument();
        var instrument2 = NewInstrument();

        var result = await _service.DeleteAsync(new DeleteByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));

        Assert.False(result.IsSucc);

        var actual = await _getByIsinsService.GetByIsinsAsync(new GetByIsinsInstrument.Command([instrument1.Isin, instrument2.Isin]));
        Assert.False(actual.IsSucc);
    }
}
