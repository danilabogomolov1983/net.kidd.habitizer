using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using PostAccount = Wst.Tools.PosiBridge.Application.Account.Post;
using GetAccount = Wst.Tools.PosiBridge.Application.Account.Get;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Account.Post;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly PostAccount.Service _postService = fixture.GetRequiredService<PostAccount.Service>();
    
    private readonly GetAccount.Service _getService = fixture.GetRequiredService<GetAccount.Service>();

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
