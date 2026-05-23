using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using GetBySourcesAccount = Net.Kidd.Habitizer.Application.Account.Bulk.GetBySources;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;


namespace Net.Kidd.Habitizer.Application.IntegrationTest.Account.Bulk.GetBySources;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetBySourcesAccount.Service _service = fixture.GetRequiredService<GetBySourcesAccount.Service>();
    private readonly AccountAddMissing.Service _addMissingAccount = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();


    [Fact]
    public async Task GetBySourcesAsync_ReturnsAccountsFromBulkStore()
    {
        var source1 = NewSource();
        var source2 = NewSource();
        
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var account1 = NewAccount() with { Source = source1 };
        var account2 = NewAccount() with { Source = source2 };

        Assert.True((await _addMissingAccount.AddMissingAsync(new AccountAddMissing.Command([account1, account2]))).IsSucc);

        var actual = await _service.GetBySourcesAsync(new GetBySourcesAccount.Command([source1, source2]));
        Assert.True(actual.IsSucc);
        var result = actual.ToOption().ValueUnsafe(); 

        Assert.Equal(2, result.Count);
        Assert.Contains(account1, result);
        Assert.Contains(account2, result);
    }
}
