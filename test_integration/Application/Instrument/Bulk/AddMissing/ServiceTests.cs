using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using InstrumentAddMissing = Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;
using GetByIsinsInstrument = Net.Kidd.Habitizer.Application.Instrument.Bulk.GetByIsins;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Instrument.Bulk.AddMissing;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly InstrumentAddMissing.Service _service = fixture.GetRequiredService<InstrumentAddMissing.Service>();
    private readonly GetByIsinsInstrument.Service _getByIsinsService = fixture.GetRequiredService<GetByIsinsInstrument.Service>();

    [Fact]
    public async Task AddMissingAsync_ExistingInstrumentsExcludedFromInsert()
    {
        var existingInstrument = NewInstrument();
        var missingInstrument = NewInstrument();

        Assert.True((await _service.AddMissingAsync(new InstrumentAddMissing.Command([existingInstrument]))).IsSucc);

        Assert.False((await _getByIsinsService.GetByIsinsAsync(new GetByIsinsInstrument.Command([existingInstrument.Isin, missingInstrument.Isin]))).IsSucc);

        var result = await _service.AddMissingAsync(new InstrumentAddMissing.Command([existingInstrument, missingInstrument]));
        Assert.True(result.IsSucc);

        var actual = await _getByIsinsService.GetByIsinsAsync(new GetByIsinsInstrument.Command([existingInstrument.Isin, missingInstrument.Isin]));
        Assert.True(actual.IsSucc);
        var actualInstruments = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualInstruments.Count);
        Assert.Contains(existingInstrument, actualInstruments);
        Assert.Contains(missingInstrument, actualInstruments);
    }
}
