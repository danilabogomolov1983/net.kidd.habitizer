using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using AccountGetByIds = Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;
using UpdateAccount = Wst.Tools.PosiBridge.Application.Account.Bulk.Update;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Account.Bulk.Update;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly UpdateAccount.Service _updateAccountService = fixture.GetRequiredService<UpdateAccount.Service>();
    private readonly AccountGetByIds.Service _getByIdsService = fixture.GetRequiredService<AccountGetByIds.Service>();
    private readonly AccountAddMissing.Service _addMissingAccountService = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly SourceAddMissing.Service _sourceAddMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();

    [Fact]
    public async Task UpdateAsync_UpdatesAccountsUsingBulkStore()
    {
        var source = NewSource();
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source]))).IsSucc);

        var initialAccount = NewAccount() with { Source = source };
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([initialAccount]))).IsSucc);

        var updatedAt = DateTimeOffset.UtcNow;
        var updatedAccount = initialAccount with { LastUpdatedAt = updatedAt };

        var result = await _updateAccountService.UpdateAsync(new UpdateAccount.Command(source.Name, [updatedAccount]));

        Assert.True(result.IsSucc);

        var actual = await _getByIdsService.GetByIdsAsync(new AccountGetByIds.Command([updatedAccount.Id]));
        Assert.True(actual.IsSucc);
        var storedAccount = Assert.Single(actual.ToOption().ValueUnsafe());

        Assert.Equal(updatedAccount, storedAccount);
        Assert.NotEqual(initialAccount, storedAccount);
    }

    [Fact]
    public async Task UpdateAsync_MissingAccount_Fails()
    {
        var source = NewSource();
        Assert.True((await _sourceAddMissingService.AddMissingAsync(new SourceAddMissing.Command([source]))).IsSucc);

        var account = NewAccount() with { Source = source };

        var result = await _updateAccountService.UpdateAsync(new UpdateAccount.Command(source.Name, [account]));

        Assert.False(result.IsSucc);
    }
}
