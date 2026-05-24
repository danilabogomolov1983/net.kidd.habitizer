using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using GetBySourcesAccount = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySources;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;


namespace Net.Kidd.Habitizer.Features.Test.Account.Bulk.GetBySources;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly GetBySourcesAccount.Service _service = new(new(new Persistence.Source.BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccount = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));


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
    
    [Fact]
    public async Task GetBySourcesAsync_AccountsDoNotExist()
    {
        var source = NewSource();

        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source]))).IsSucc);

        var account1 = NewAccount() with { Source = source };
        var account2 = NewAccount() with { Source = source };

        var actual = await _service.GetBySourcesAsync(new GetBySourcesAccount.Command([source]));

        Assert.True(actual.IsFail);
    }
}
