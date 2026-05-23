using Net.Kidd.Habitizer.TestCompanion;
using DeleteAccount = Net.Kidd.Habitizer.Application.Account.Bulk.Delete;
using AccountGetByIds = Net.Kidd.Habitizer.Application.Account.Bulk.GetByIds;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Account.Bulk.Delete;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly DeleteAccount.Service _deleteAccountService = fixture.GetRequiredService<DeleteAccount.Service>();
    private readonly AccountGetByIds.Service _getByIdsService = fixture.GetRequiredService<AccountGetByIds.Service>();
    private readonly AccountAddMissing.Service _addMissingAccount = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();

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
