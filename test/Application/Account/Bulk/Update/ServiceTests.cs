using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using AccountGetByIds = Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;
using UpdateAccount = Wst.Tools.PosiBridge.Application.Account.Bulk.Update;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.Test.Account.Bulk.Update;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly UpdateAccount.Service _updateAccountService = new(new BulkStore(fixture.ContextFactory));
    private readonly AccountGetByIds.Service _getByIdsService = new(new BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccountService = new(new BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _sourceAddMissingService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));

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
