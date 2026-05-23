using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Source;

[Collection("IntegrationTests")]
public class BulkOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IBulkStore _bulkStore = new BulkStore(fixture.ContextFactory);
    private readonly Domain.Account.IBulkStore _accountBulkStore = new Persistence.Account.BulkStore(fixture.ContextFactory);

    [Fact]
    public async Task Source_Insert_AddsOnlyMissingSources()
    {
        var expected = NewSource();
        Assert.True((await _bulkStore.InsertAsync([expected])).IsSucc);

        var actual = await _bulkStore.GetByNamesAsync([expected.Name]);

        Assert.Single(actual);
        Assert.Contains(expected, actual);
    }

    [Fact]
    public async Task Source_DeleteByNames_RemovesOnlyMatchingSources()
    {
        var deleted = NewSource();
        var kept = NewSource();

        Assert.True((await _bulkStore.InsertAsync([deleted, kept])).IsSucc);

        Assert.True((await _bulkStore.DeleteByNamesAsync([deleted.Name])).IsSucc);

        var deletedSources = await _bulkStore.GetByNamesAsync([deleted.Name]);
        var keptSources = await _bulkStore.GetByNamesAsync([kept.Name]);

        Assert.Empty(deletedSources);
        Assert.Single(keptSources);
        Assert.Contains(kept, keptSources);
    }

    [Fact]
    public async Task Source_DeleteByNames_CascadesToAccounts()
    {
        var deletedSource = NewSource();
        var keptSource = NewSource();

        Assert.True((await _bulkStore.InsertAsync([deletedSource])).IsSucc);
        Assert.True((await _bulkStore.InsertAsync([keptSource])).IsSucc);

        var deletedAccount = NewAccount() with { Source = deletedSource };
        var keptAccount = NewAccount() with { Source = keptSource };

        Assert.True((await _accountBulkStore.InsertAsync([deletedAccount, keptAccount])).IsSucc);

        Assert.True((await _bulkStore.DeleteByNamesAsync([deletedSource.Name])).IsSucc);

        var deletedAccounts = await _accountBulkStore.GetBySourcesAsync([deletedSource]);
        var keptAccounts = await _accountBulkStore.GetBySourcesAsync([keptSource]);

        Assert.Empty(deletedAccounts);
        Assert.Single(keptAccounts);
        Assert.Contains(keptAccount, keptAccounts);
    }
}

