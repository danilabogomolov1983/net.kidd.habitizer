using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using PostAccount = Net.Kidd.Habitizer.Application.Account.Post;
using GetAccount = Net.Kidd.Habitizer.Application.Account.Get;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Account.Get;

[Collection("IntegrationTests")]
public class GetAccountTests(IntegrationTestsFixture fixture)
{
    private readonly PostAccount.Service _postService = fixture.GetRequiredService<PostAccount.Service>();
    private readonly GetAccount.Service _getService = fixture.GetRequiredService<GetAccount.Service>();

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
