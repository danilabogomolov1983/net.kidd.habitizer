using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using static Wst.Tools.PosiBridge.Domain.Position.Extensions;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Account;

[Collection("IntegrationTests")]
public class BulkOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly Domain.Source.IBulkStore _sourceBulkStore = new Persistence.Source.BulkStore(fixture.ContextFactory); 
    private readonly Domain.Instrument.IBulkStore _instrumentBulkStore = new Persistence.Instrument.BulkStore(fixture.ContextFactory); 
    private readonly Domain.Position.IBulkStore _positionBulkStore = new Persistence.Position.BulkStore(fixture.ContextFactory);

    private readonly IBulkStore _bulkStore = new BulkStore(fixture.ContextFactory);

    [Fact]
    public async Task Account_Insert_AddsOnlyMissingAccounts()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        Assert.True((await _bulkStore.InsertAsync([account1, account2])).IsSucc);

        var actual = await _bulkStore.GetBySourcesAsync([source]);

        Assert.Equal(2, actual.Count);
        Assert.Contains(account1, actual);
        Assert.Contains(account2, actual);
    }

    [Fact]
    public async Task Account_GetByIds_ReturnsMatchingAccountsAcrossSources()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var instrument = NewInstrument();
        Assert.True((await _instrumentBulkStore.InsertAsync([instrument])).IsSucc);
        
        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        Assert.True((await _bulkStore.InsertAsync([account1, account2])).IsSucc);

        var actual = await _bulkStore.GetByIdsAsync([account1.Id, account2.Id]);

        Assert.Equal(2, actual.Count);
        Assert.Contains(account1, actual);
        Assert.Contains(account2, actual);
    }

    [Fact]
    public async Task Account_GetBySourceAndNames_ReturnsOnlyMatchingAccountsForSource()
    {
        var source = NewSource();
        var otherSource = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source, otherSource])).IsSucc);

        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        var sameNameDifferentSource = account1 with { Id = AccountId.New(), Source = otherSource };
        Assert.True((await _bulkStore.InsertAsync([account1, account2, sameNameDifferentSource])).IsSucc);

        var actual = await _bulkStore.GetBySourceAndNamesAsync(source.Name, [account1.Name, account2.Name]);

        Assert.Equal(2, actual.Count);
        Assert.Contains(account1, actual);
        Assert.Contains(account2, actual);
        Assert.DoesNotContain(sameNameDifferentSource, actual);
    }

    [Fact]
    public async Task Account_Update_UpdatesMatchingAccounts()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var original = NewAccount().WithSource(source).WithLastUpdatedAt(DateTimeOffset.UtcNow.AddMinutes(-10));
        Assert.True((await _bulkStore.InsertAsync([original])).IsSucc);

        var updated = original
            .WithLastUpdatedAt(DateTimeOffset.UtcNow);

        Assert.True((await _bulkStore.UpdateAsync([updated])).IsSucc);

        var originalSourceAccounts = await _bulkStore.GetBySourcesAsync([source]);

        Assert.Single(originalSourceAccounts);
        Assert.Contains(updated, originalSourceAccounts);
    }
    
    [Fact]
    public async Task Account_Update_UpdatesNonExistingAccounts()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var original = NewAccount().WithSource(source).WithLastUpdatedAt(DateTimeOffset.UtcNow.AddMinutes(-10));

        var updated = original.WithLastUpdatedAt(DateTimeOffset.UtcNow);

        Assert.False((await _bulkStore.UpdateAsync([updated])).IsSucc);
    }

    [Fact]
    public async Task Account_Delete_RemovesMatchingAccounts()
    {
        var source = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);

        var account1 = NewAccount().WithSource(source);
        var account2 = NewAccount().WithSource(source);
        var kept = NewAccount().WithSource(source);
        
        Assert.True((await _bulkStore.InsertAsync([account1, account2, kept])).IsSucc);

        Assert.True((await _bulkStore.DeleteAsync([account1, account2])).IsSucc);

        var deletedAccounts = await _bulkStore.GetByIdsAsync([account1.Id, account2.Id]);
        var keptAccounts = await _bulkStore.GetByIdsAsync([kept.Id]);

        Assert.Empty(deletedAccounts);
        Assert.Single(keptAccounts);
        Assert.Contains(kept, keptAccounts);
    }

    [Fact]
    public async Task Account_DeleteBySource_RemovesOnlyMatchingAccounts()
    {
        var targetSource = NewSource();
        var otherSource = NewSource();
        
        Assert.True((await _sourceBulkStore.InsertAsync([targetSource])).IsSucc);
        Assert.True((await _sourceBulkStore.InsertAsync([otherSource])).IsSucc);
        
        var deleted1 = NewAccount() with { Source = targetSource };
        var deleted2 = NewAccount() with { Source = targetSource };
        var kept = NewAccount() with { Source = otherSource };

        Assert.True((await _bulkStore.InsertAsync([deleted1, deleted2])).IsSucc);
        Assert.True((await _bulkStore.InsertAsync([kept])).IsSucc);

        Assert.True((await _bulkStore.DeleteBySourceAsync([targetSource])).IsSucc);

        var targetAccounts = await _bulkStore.GetBySourcesAsync([targetSource]);
        var otherAccounts = await _bulkStore.GetBySourcesAsync([otherSource]);

        Assert.Empty(targetAccounts);
        Assert.Single(otherAccounts);
        Assert.Contains(kept, otherAccounts);
    }

    [Fact]
    public async Task Account_DeleteByIds_RemovesOnlyMatchingAccounts()
    {
        var source = NewSource();
        
        Assert.True((await _sourceBulkStore.InsertAsync([source])).IsSucc);
        
        var deleted1 = NewAccount() with { Source = source };
        var deleted2 = NewAccount() with { Source = source };
        var kept = NewAccount() with { Source = source };

        Assert.True((await _bulkStore.InsertAsync([deleted1, deleted2])).IsSucc);
        Assert.True((await _bulkStore.InsertAsync([kept])).IsSucc);

        Assert.True((await _bulkStore.DeleteByIdsAsync([deleted1.Id, deleted2.Id])).IsSucc);

        var accounts = await _bulkStore.GetBySourcesAsync([source]);

        Assert.Single(accounts);
        Assert.Contains(kept, accounts);
    }

    [Fact]
    public async Task Account_Delete_CascadesToPositions()
    {
        var deletedSource = NewSource();
        var keptSource = NewSource();
        Assert.True((await _sourceBulkStore.InsertAsync([deletedSource])).IsSucc);
        Assert.True((await _sourceBulkStore.InsertAsync([keptSource])).IsSucc);
        
        var instrument = NewInstrument();
        Assert.True((await _instrumentBulkStore.InsertAsync([instrument])).IsSucc);

        var deletedAccount = NewAccount().WithSource(deletedSource);
        var keptAccount = NewAccount().WithSource(keptSource);
        Assert.True((await _bulkStore.InsertAsync([deletedAccount])).IsSucc);
        Assert.True((await _bulkStore.InsertAsync([keptAccount])).IsSucc);
        
        var deletedPosition = NewPosition().WithAccount(deletedAccount).WithInstrument(instrument);
        var keptPosition = NewPosition().WithAccount(keptAccount).WithInstrument(instrument);

        Assert.True((await _positionBulkStore.InsertAsync([deletedPosition, keptPosition])).IsSucc);

        Assert.True((await _bulkStore.DeleteBySourceAsync([deletedSource])).IsSucc);

        var deletedPositions = await _positionBulkStore.GetByAccountIdsAsync([deletedAccount.Id]);
        var keptPositions = await _positionBulkStore.GetByAccountIdsAsync([keptAccount.Id]);

        Assert.Empty(deletedPositions);
        Assert.Single(keptPositions);
        Assert.Contains(keptPosition, keptPositions);
    }
}
