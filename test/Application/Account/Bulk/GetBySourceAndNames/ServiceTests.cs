using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;
using AccountGetBySourceAndNames = Net.Kidd.Habitizer.Application.Account.Bulk.GetBySourceAndNames;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Application.Test.Account.Bulk.GetBySourceAndNames;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly AccountGetBySourceAndNames.Service _service = new(new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccount = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task GetBySourceAndNamesAsync_ReturnsOnlyMatchingAccountsForSource()
    {
        var source = NewSource();
        var otherSource = NewSource();

        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source, otherSource]))).IsSucc);

        var account1 = NewAccount() with { Source = source };
        var account2 = NewAccount() with { Source = source };
        var sameNameDifferentSource = account1 with { Id = Domain.Account.AccountId.New(), Source = otherSource };

        Assert.True((await _addMissingAccount.AddMissingAsync(new AccountAddMissing.Command([account1, account2, sameNameDifferentSource]))).IsSucc);

        var actual = await _service.GetBySourceAndNamesAsync(new AccountGetBySourceAndNames.Command(source.Name, [account1.Name, account2.Name]));

        Assert.True(actual.IsSucc);
        var result = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, result.Count);
        Assert.Contains(account1, result);
        Assert.Contains(account2, result);
        Assert.DoesNotContain(sameNameDifferentSource, result);
    }

    [Fact]
    public async Task GetBySourceAndNamesAsync_AccountsDoNotExist()
    {
        var source = NewSource();

        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source]))).IsSucc);

        var account1 = NewAccount() with { Source = source };
        var account2 = NewAccount() with { Source = source };

        var actual = await _service.GetBySourceAndNamesAsync(new AccountGetBySourceAndNames.Command(source.Name, [account1.Name, account2.Name]));

        Assert.True(actual.IsFail);
    }
}
