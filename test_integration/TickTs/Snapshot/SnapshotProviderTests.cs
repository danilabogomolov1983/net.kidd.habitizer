    using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Account;

namespace Net.Kidd.Habitizer.TickTs.IntegrationTest.Snapshot;

[Collection("IntegrationTests")]
public class SnapshotProviderTests(TickTsFixture fixture, ITestOutputHelper output)
{
    private readonly IPortfolioSnapshotProvider _provider = fixture.SnapshotProvider;

    [Fact]
    public async Task Get_Success()
    {
        var maybeActual = await _provider.GetAsync("7873");

        maybeActual.IfFail(err =>
        {
            output.WriteLine(err.Message);
        });
        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Positions);
    }

    [Fact]
    public async Task Get_Account_Does_Not_Exist()
    {
        var maybeActual = await _provider.GetAsync(Account.Empty().Name);
        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Positions);        
    }

    [Fact]
    public async Task Get_Multiple_Accounts_Returns_Combined_Snapshot()
    {
        var account1 = "7873";
        var account2 = "7814_Renten2";
        var maybeActual = await _provider.GetAsync([account1, account2]);

        maybeActual.IfFail(err => output.WriteLine(err.Message));
        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Positions);

        var actualAccounts = actual.Positions.Select(i => i.Account.Name.Value).ToList();
        Assert.Contains(account1, actualAccounts);
        Assert.Contains(account2, actualAccounts);

    }
}
