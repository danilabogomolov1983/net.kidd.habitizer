using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using DeleteBySourceAccount = Net.Kidd.Habitizer.Features.Account.Bulk.DeleteBySource;
using GetBySourcesAccount = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySources;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Account.Bulk.DeleteBySource;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly DeleteBySourceAccount.Service _service = fixture.GetRequiredService<DeleteBySourceAccount.Service>();
    private readonly GetBySourcesAccount.Service _getBySourcesService = fixture.GetRequiredService<GetBySourcesAccount.Service>();
    private readonly AccountAddMissing.Service _addMissingAccount = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();


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
