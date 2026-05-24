using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using AccountAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;
using AccountGetBySourceAndNames = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySourceAndNames;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Account.Bulk.GetBySourceAndNames;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly AccountGetBySourceAndNames.Service _service = fixture.GetRequiredService<AccountGetBySourceAndNames.Service>();
    private readonly AccountAddMissing.Service _addMissingAccount = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();

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
