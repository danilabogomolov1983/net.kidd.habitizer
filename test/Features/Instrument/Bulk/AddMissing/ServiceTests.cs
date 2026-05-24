using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.TestCompanion;
using InstrumentAddMissing = Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;
using GetByIsinsInstrument = Net.Kidd.Habitizer.Features.Instrument.Bulk.GetByIsins;

namespace Net.Kidd.Habitizer.Features.Test.Instrument.Bulk.AddMissing;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly InstrumentAddMissing.Service _service = new(new BulkStore(fixture.ContextFactory));
    private readonly GetByIsinsInstrument.Service _getByIsinsService = new(new BulkStore(fixture.ContextFactory));

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

    [Fact]
    public async Task AddMissingAsync_AllInstrumentsAlreadyExist_ReturnsSuccessWithoutChanges()
    {
        var existingInstrument1 = NewInstrument();
        var existingInstrument2 = NewInstrument();

        Assert.True((await _service.AddMissingAsync(new InstrumentAddMissing.Command([existingInstrument1, existingInstrument2]))).IsSucc);

        var result = await _service.AddMissingAsync(new InstrumentAddMissing.Command([existingInstrument1, existingInstrument2]));
        Assert.True(result.IsSucc);

        var actual = await _getByIsinsService.GetByIsinsAsync(new GetByIsinsInstrument.Command([existingInstrument1.Isin, existingInstrument2.Isin]));
        Assert.True(actual.IsSucc);
        var actualInstruments = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualInstruments.Count);
        Assert.Contains(existingInstrument1, actualInstruments);
        Assert.Contains(existingInstrument2, actualInstruments);
    }
}
