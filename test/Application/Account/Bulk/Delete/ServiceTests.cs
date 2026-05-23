using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using DeleteAccount = Net.Kidd.Habitizer.Application.Account.Bulk.Delete;
using AccountGetByIds = Net.Kidd.Habitizer.Application.Account.Bulk.GetByIds;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Application.Test.Account.Bulk.Delete;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly DeleteAccount.Service _deleteAccountService = new(new(new BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly AccountGetByIds.Service _getByIdsService = new(new BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccount = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task DeleteAsync_DeletesNamesUsingBulkStore()
    {
        var source1 = NewSource();
        var source2 = NewSource();
        
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var account1 = NewAccount() with { Source = source1 };
        var account2 = NewAccount() with { Source = source2 };

        Assert.True((await _addMissingAccount.AddMissingAsync(new AccountAddMissing.Command([account1, account2]))).IsSucc);

        var result = await _deleteAccountService.DeleteAsync(new DeleteAccount.Command([account1, account2]));

        Assert.True(result.IsSucc);

        var actual = await _getByIdsService.GetByIdsAsync(new AccountGetByIds.Command([account1.Id, account2.Id]));
        Assert.Empty(actual);
    }
}
