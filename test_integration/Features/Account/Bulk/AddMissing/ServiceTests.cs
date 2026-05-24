using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using AccountAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using AccountGetByIds = Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Account.Bulk.AddMissing;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly AccountAddMissing.Service _service = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly AccountGetByIds.Service _getByIdsService = fixture.GetRequiredService<AccountGetByIds.Service>();

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
}
