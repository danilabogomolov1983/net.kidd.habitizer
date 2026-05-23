using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.TestCompanion;
using static Net.Kidd.Habitizer.Domain.Account.Extensions;

namespace Net.Kidd.Habitizer.Persistence.Test.Position;

public class BulkOperationsTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly Domain.Account.IBulkStore _accountBulkStore = new Persistence.Account.BulkStore(fixture.ContextFactory);
    private readonly Domain.Instrument.IBulkStore _instrumentBulkStore = new Persistence.Instrument.BulkStore(fixture.ContextFactory);
    private readonly Domain.Source.IBulkStore _sourceBulkStore = new Persistence.Source.BulkStore(fixture.ContextFactory); 

    private readonly IBulkStore _bulkStore = new BulkStore(fixture.ContextFactory);

    [Fact]
    public async Task Position_Insert_AddsOnlyMissingPositions()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var instrument = NewInstrument();
        Assert.True((await _instrumentBulkStore.InsertAsync([instrument])).IsSucc);
        
        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        Assert.True((await _accountBulkStore.InsertAsync([account1, account2])).IsSucc);

        var position1 = NewPosition().WithAccount(account1).WithInstrument(instrument);
        var position2 = NewPosition().WithAccount(account2).WithInstrument(instrument);

        Assert.True((await _bulkStore.InsertAsync([position1, position2])).IsSucc);

        var actual = await _bulkStore.GetByAccountIdsAsync([account1.Id, account2.Id]);

        Assert.Equal(2, actual.Count);
        Assert.Contains(position1, actual);
        Assert.Contains(position2, actual);
    }

    [Fact]
    public async Task Position_GetBySource_ReturnsOnlyPositionsForMatchingSources()
    {
        var targetSource = NewSource();
        var otherSource = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([targetSource, otherSource])).IsSucc);

        var instruments = new[] { NewInstrument(), NewInstrument(), NewInstrument() };
        Assert.True((await _instrumentBulkStore.InsertAsync(instruments)).IsSucc);

        var targetAccount1 = NewAccount().WithSource(targetSource);
        var targetAccount2 = NewAccount().WithSource(targetSource);
        var otherAccount = NewAccount().WithSource(otherSource);
        Assert.True((await _accountBulkStore.InsertAsync([targetAccount1, targetAccount2, otherAccount])).IsSucc);

        var expected1 = NewPosition().WithAccount(targetAccount1).WithInstrument(instruments[0]);
        var expected2 = NewPosition().WithAccount(targetAccount2).WithInstrument(instruments[1]);
        var otherPosition = NewPosition().WithAccount(otherAccount).WithInstrument(instruments[2]);
        Assert.True((await _bulkStore.InsertAsync([expected1, expected2, otherPosition])).IsSucc);

        var actual = await _bulkStore.GetBySourceAsync([targetSource]);

        Assert.Equal(2, actual.Count);
        Assert.Contains(expected1, actual);
        Assert.Contains(expected2, actual);
        Assert.DoesNotContain(otherPosition, actual);
    }

    [Fact]
    public async Task Position_DeleteByAccount_RemovesOnlyPositionsForMatchingAccounts()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var instruments = new[] { NewInstrument(), NewInstrument(), NewInstrument() };
        Assert.True((await _instrumentBulkStore.InsertAsync(instruments)).IsSucc);

        var deletedAccount1 = NewAccount().WithSource(source);
        var deletedAccount2 = NewAccount().WithSource(source);
        var keptAccount = NewAccount().WithSource(source);
        Assert.True((await _accountBulkStore.InsertAsync([deletedAccount1, deletedAccount2, keptAccount])).IsSucc);

        var deletedPosition1 = NewPosition().WithAccount(deletedAccount1).WithInstrument(instruments[0]);
        var deletedPosition2 = NewPosition().WithAccount(deletedAccount2).WithInstrument(instruments[1]);
        var keptPosition = NewPosition().WithAccount(keptAccount).WithInstrument(instruments[2]);
        Assert.True((await _bulkStore.InsertAsync([deletedPosition1, deletedPosition2, keptPosition])).IsSucc);

        Assert.True((await _bulkStore.DeleteByAccountAsync([deletedAccount1, deletedAccount2])).IsSucc);

        var deletedPositions = await _bulkStore.GetByAccountIdsAsync([deletedAccount1.Id, deletedAccount2.Id]);
        var keptPositions = await _bulkStore.GetByAccountIdsAsync([keptAccount.Id]);

        Assert.Empty(deletedPositions);
        Assert.Single(keptPositions);
        Assert.Contains(keptPosition, keptPositions);
    }

    [Fact]
    public async Task Position_DeleteBySource_RemovesOnlyPositionsForMatchingSource()
    {
        var deletedSource = NewSource();
        var keptSource = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([deletedSource, keptSource])).IsSucc);

        var instruments = new[] { NewInstrument(), NewInstrument(), NewInstrument() };
        Assert.True((await _instrumentBulkStore.InsertAsync(instruments)).IsSucc);

        var deletedAccount1 = NewAccount().WithSource(deletedSource);
        var deletedAccount2 = NewAccount().WithSource(deletedSource);
        var keptAccount = NewAccount().WithSource(keptSource);
        Assert.True((await _accountBulkStore.InsertAsync([deletedAccount1, deletedAccount2, keptAccount])).IsSucc);

        var deletedPosition1 = NewPosition().WithAccount(deletedAccount1).WithInstrument(instruments[0]);
        var deletedPosition2 = NewPosition().WithAccount(deletedAccount2).WithInstrument(instruments[1]);
        var keptPosition = NewPosition().WithAccount(keptAccount).WithInstrument(instruments[2]);
        Assert.True((await _bulkStore.InsertAsync([deletedPosition1, deletedPosition2, keptPosition])).IsSucc);

        Assert.True((await _bulkStore.DeleteBySourceAsync([deletedSource])).IsSucc);

        var deletedPositions = await _bulkStore.GetBySourceAsync([deletedSource]);
        var keptPositions = await _bulkStore.GetBySourceAsync([keptSource]);

        Assert.Empty(deletedPositions);
        Assert.Single(keptPositions);
        Assert.Contains(keptPosition, keptPositions);
    }

    [Fact]
    public async Task Position_Insert_WithoutPersistedAccounts_ReturnsFailedFin()
    {
        var instrument = NewInstrument();
        Assert.True((await _instrumentBulkStore.InsertAsync([instrument])).IsSucc);

        var position = NewPosition().WithInstrument(instrument);

        var actual = await _bulkStore.InsertAsync([position]);

        Assert.False(actual.IsSucc);
    }

    [Fact]
    public async Task Position_Insert_WithoutPersistedInstruments_ReturnsFailedFin()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var account = NewAccount().WithSource(source);
        Assert.True((await _accountBulkStore.InsertAsync([account])).IsSucc);

        var position = NewPosition().WithAccount(account);

        var actual = await _bulkStore.InsertAsync([position]);

        Assert.False(actual.IsSucc);
    }
}
