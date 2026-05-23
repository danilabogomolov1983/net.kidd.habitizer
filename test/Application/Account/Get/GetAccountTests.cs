using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Application.Source.Post;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using PostAccount = Net.Kidd.Habitizer.Application.Account.Post;
using GetAccount = Net.Kidd.Habitizer.Application.Account.Get;

namespace Net.Kidd.Habitizer.Application.Test.Account.Get;

public class GetAccountTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostAccount.Service _postService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))
    );
    private readonly GetAccount.Service _getService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))
    );

    [Fact]
    public async Task Get_Success()
    {
        var account = NewAccount();
        var postCommand = new PostAccount.Command(account.Source.Name, account.Name);

        var maybePosted = await _postService.PostAsync(postCommand);
        Assert.True(maybePosted.IsSucc);
        var expected = maybePosted.ToOption().ValueUnsafe();

        var getCommand = new GetAccount.Command(expected.Source.Name, expected.Name);
        var maybeAccount = await _getService.GetAsync(getCommand);
        Assert.True(maybeAccount.IsSucc);
        var actual = maybeAccount.ToOption().ValueUnsafe();
        
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }
}
