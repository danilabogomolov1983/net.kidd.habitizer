using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Application.Source.Post;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using PostAccount = Wst.Tools.PosiBridge.Application.Account.Post;
using GetListAccount = Wst.Tools.PosiBridge.Application.Account.GetList;

namespace Wst.Tools.PosiBridge.Application.Test.Account.GetList;

public class GetListTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostAccount.Service _postService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))
    );
    private readonly GetListAccount.Service _getListService = new(new PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Get_List_Success()
    {
        var expected1 = NewAccount();
        var expected2 = NewAccount();

        var postResult1 = await _postService.PostAsync(new PostAccount.Command(expected1.Source.Name, expected1.Name));
        Assert.True(postResult1.IsSucc);
        var posted1 = Assert.IsType<Domain.Account.Account>(postResult1.ToOption().ValueUnsafe());
        Assert.NotNull(posted1);

        var postResult2 = await _postService.PostAsync(new PostAccount.Command(expected2.Source.Name, expected2.Name));
        Assert.True(postResult2.IsSucc);
        var posted2 = Assert.IsType<Domain.Account.Account>(postResult2.ToOption().ValueUnsafe());
        Assert.NotNull(posted2);

        var getListCommand = new GetListAccount.Command(1, 10);
        var actual = await _getListService.GetListAsync(getListCommand);
        Assert.Contains(actual, item =>
            item.Id == posted1.Id &&
            item.Name == expected1.Name &&
            item.Source.Name == expected1.Source.Name);
        Assert.Contains(actual, item =>
            item.Id == posted2.Id &&
            item.Name == expected2.Name &&
            item.Source.Name == expected2.Source.Name);
    }
}
