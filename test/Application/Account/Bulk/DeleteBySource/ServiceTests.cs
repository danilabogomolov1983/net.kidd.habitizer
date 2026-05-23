using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using DeleteBySourceAccount = Wst.Tools.PosiBridge.Application.Account.Bulk.DeleteBySource;
using GetBySourcesAccount = Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySources;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.Test.Account.Bulk.DeleteBySource;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly DeleteBySourceAccount.Service _service = new(new(new Persistence.Source.BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly GetBySourcesAccount.Service _getBySourcesService = new(new(new Persistence.Source.BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccount = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));


    [Fact]
    public async Task DeleteAsync_DeletesSourcesUsingBulkStore()
    {
        var source1 = NewSource();
        var source2 = NewSource();
        
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var account1 = NewAccount() with { Source = source1 };
        var account2 = NewAccount() with { Source = source2 };
        Assert.True((await _addMissingAccount.AddMissingAsync(new AccountAddMissing.Command([account1, account2]))).IsSucc);
        
        Assert.True((await _service.DeleteAsync(new DeleteBySourceAccount.Command([source1]))).IsSucc);
        
        var actualDeletedAccounts = await _getBySourcesService.GetBySourcesAsync(new GetBySourcesAccount.Command([source1]));
        Assert.False(actualDeletedAccounts.IsSucc);
        
        var actualRemainingAccounts = await _getBySourcesService.GetBySourcesAsync(new GetBySourcesAccount.Command([source2]));
        Assert.True(actualRemainingAccounts.IsSucc);
        var remainingAccounts = actualRemainingAccounts.ToOption().ValueUnsafe();

        Assert.Single(remainingAccounts);
        Assert.Contains(account2, remainingAccounts);
    }
}
