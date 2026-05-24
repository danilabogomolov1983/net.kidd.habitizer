using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using GetByIsinsInstrument = Net.Kidd.Habitizer.Features.Instrument.Bulk.GetByIsins;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Instrument.Bulk.GetByIsins;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly Domain.Instrument.IBulkStore _bulkStore = fixture.GetRequiredService<Domain.Instrument.IBulkStore>();
    private readonly GetByIsinsInstrument.Service _service = fixture.GetRequiredService<GetByIsinsInstrument.Service>();

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
