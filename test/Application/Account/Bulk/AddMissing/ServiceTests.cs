using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using AccountGetByIds = Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;

namespace Wst.Tools.PosiBridge.Application.Test.Account.Bulk.AddMissing;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly AccountAddMissing.Service _service = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));
    private readonly AccountGetByIds.Service _getByIdsService = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task AddMissingAsync_ExistingAccountsExcludedFromAdd()
    {
        var source1 = NewSource();
        var source2 = NewSource();
        
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var account1 = NewAccount() with { Source = source1 };
        var account2 = NewAccount() with { Source = source2 };

        Assert.True((await _service.AddMissingAsync(new AccountAddMissing.Command([account1, account2]))).IsSucc);
        
        var actualExistingAccounts = await _getByIdsService.GetByIdsAsync(new AccountGetByIds.Command([account1.Id, account2.Id]));
        Assert.True(actualExistingAccounts.IsSucc);
        var existingAccounts = actualExistingAccounts.ToOption().ValueUnsafe();
        
        Assert.NotEmpty(existingAccounts);
        Assert.Contains(account1, existingAccounts);
        Assert.Contains(account2, existingAccounts);
        
    }
    
    [Fact]
    public async Task AddMissingAsync_AccountIsMissing()
    {
        var source1 = NewSource();
        var source2 = NewSource();

        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var existingAccount = NewAccount() with { Source = source1 };
        var missingAccount = NewAccount() with { Source = source2 };

        Assert.True((await _service.AddMissingAsync(new AccountAddMissing.Command([existingAccount]))).IsSucc);
        
        var actualExistingAccounts = await _getByIdsService.GetByIdsAsync(new AccountGetByIds.Command([existingAccount.Id, missingAccount.Id]));
        Assert.False(actualExistingAccounts.IsSucc);
    }

    [Fact]
    public async Task AddMissingAsync_SameAccountNameInDifferentSources_AddsBothAccounts()
    {
        var source1 = NewSource();
        var source2 = NewSource();
        var accountName = NewAccountName();

        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var account1 = NewAccount() with { Source = source1, Name = accountName };
        var account2 = NewAccount() with { Source = source2, Name = accountName };

        var result = await _service.AddMissingAsync(new AccountAddMissing.Command([account1, account2]));
        Assert.True(result.IsSucc);

        var actualExistingAccounts = await _getByIdsService.GetByIdsAsync(new AccountGetByIds.Command([account1.Id, account2.Id]));
        Assert.True(actualExistingAccounts.IsSucc);
        var existingAccounts = actualExistingAccounts.ToOption().ValueUnsafe();

        Assert.Equal(2, existingAccounts.Count);
        Assert.Contains(account1, existingAccounts);
        Assert.Contains(account2, existingAccounts);
    }
}
