using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.TestCompanion;
using static Net.Kidd.Habitizer.Domain.Position.Extensions;
using static Net.Kidd.Habitizer.Domain.Account.Extensions;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Instrument;

[Collection("IntegrationTests")]
public class BulkOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IBulkStore _bulkStore = new BulkStore(fixture.ContextFactory);
    private readonly Domain.Source.IBulkStore _sourceBulkStore = new Persistence.Source.BulkStore(fixture.ContextFactory);
    private readonly Domain.Account.IBulkStore _accountBulkStore = new Persistence.Account.BulkStore(fixture.ContextFactory);
    private readonly Domain.Position.IBulkStore _positionBulkStore = new Persistence.Position.BulkStore(fixture.ContextFactory);
    [Fact]
    public async Task Instrument_Insert_AddsOnlyMissingInstruments()
    {
        var missing = NewInstrument();
        Assert.True((await _bulkStore.InsertAsync([missing])).IsSucc);

        var actual = await _bulkStore.GetByIsinsAsync([missing.Isin]);

        Assert.Single(actual);
        Assert.Contains(missing, actual);
    }

    [Fact]
    public async Task Instrument_Delete_RemovesOnlyMatchingInstruments()
    {
        var deleted = NewInstrument();
        var kept = NewInstrument();

        Assert.True((await _bulkStore.InsertAsync([deleted, kept])).IsSucc);

        Assert.True((await _bulkStore.DeleteByIsinsAsync([deleted.Isin])).IsSucc);

        var deletedInstruments = await _bulkStore.GetByIsinsAsync([deleted.Isin]);
        var keptInstruments = await _bulkStore.GetByIsinsAsync([kept.Isin]);

        Assert.Empty(deletedInstruments);
        Assert.Single(keptInstruments);
        Assert.Contains(kept, keptInstruments);
    }

    [Fact]
    public async Task Instrument_Delete_ReferencedByPosition_ReturnsFailedFin_AndKeepsInstrument()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var instrument = NewInstrument();
        Assert.True((await _bulkStore.InsertAsync([instrument])).IsSucc);
        
        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        Assert.True((await _accountBulkStore.InsertAsync([account1, account2])).IsSucc);

        var position1 = NewPosition().WithAccount(account1).WithInstrument(instrument);
        var position2 = NewPosition().WithAccount(account2).WithInstrument(instrument);

        Assert.True((await _positionBulkStore.InsertAsync([position1, position2])).IsSucc);

        Assert.False((await _bulkStore.DeleteByIsinsAsync([instrument.Isin])).IsSucc);

        var instruments = await _bulkStore.GetByIsinsAsync([instrument.Isin]);
        var positions = await _positionBulkStore.GetByAccountIdsAsync([account1.Id, account2.Id]);

        Assert.Single(instruments);
        Assert.Contains(instrument, instruments);
        Assert.Contains(position1, positions);
        Assert.Contains(position2, positions);
    }
}

