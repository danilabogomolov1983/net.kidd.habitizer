using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Features.Source.Post;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using PostAccount = Net.Kidd.Habitizer.Features.Account.Post;
using GetAccount = Net.Kidd.Habitizer.Features.Account.Get;

namespace Net.Kidd.Habitizer.Features.Test.Account.Post;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{

    private readonly PostAccount.Service _postService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))
        );
    
    private readonly GetAccount.Service _getService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Features.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))
    );

    [Fact]
    public async Task Post_Get_Success()
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
    
    [Fact]
    public async Task Post_TwoAccountsAndOneSource_Get_Success()
    {
        var source = NewSource();
        var account1 = NewAccount() with{ Source = source };
        var account2 = NewAccount() with{ Source = source };
        var postCommand1 = new PostAccount.Command(account1.Source.Name, account1.Name);
        var postCommand2 = new PostAccount.Command(account2.Source.Name, account2.Name);

        var maybePosted1 = await _postService.PostAsync(postCommand1);
        var maybePosted2 = await _postService.PostAsync(postCommand2);
        Assert.True(maybePosted1.IsSucc);
        Assert.True(maybePosted2.IsSucc);
        var expected1 = maybePosted1.ToOption().ValueUnsafe();
        var expected2 = maybePosted1.ToOption().ValueUnsafe();

        var getCommand1 = new GetAccount.Command(expected1.Source.Name, expected1.Name);
        var getCommand2 = new GetAccount.Command(expected2.Source.Name, expected2.Name);
        
        var maybeAccount1 = await _getService.GetAsync(getCommand1);
        var maybeAccount2 = await _getService.GetAsync(getCommand2);
        
        Assert.True(maybeAccount1.IsSucc);
        var actual1 = maybeAccount1.ToOption().ValueUnsafe();
        Assert.True(maybeAccount2.IsSucc);
        var actual2 = maybeAccount2.ToOption().ValueUnsafe();
        
        Assert.NotNull(actual1);
        Assert.Equal(expected1, actual1);

        Assert.NotNull(actual2);
        Assert.Equal(expected2, actual2);
    }
}
