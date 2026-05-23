using Wst.Tools.PosiBridge.TestCompanion;
using DeleteByIsinsInstrument = Wst.Tools.PosiBridge.Application.Instrument.Bulk.DeleteByIsins;
using GetByIsinsInstrument = Wst.Tools.PosiBridge.Application.Instrument.Bulk.GetByIsins;
using InstrumentAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Instrument.Bulk.DeleteByIsins;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly InstrumentAddMissing.Service _addMissingService = fixture.GetRequiredService<InstrumentAddMissing.Service>();
    private readonly DeleteByIsinsInstrument.Service _service = fixture.GetRequiredService<DeleteByIsinsInstrument.Service>();
    private readonly GetByIsinsInstrument.Service _getByIsinsService = fixture.GetRequiredService<GetByIsinsInstrument.Service>();

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
