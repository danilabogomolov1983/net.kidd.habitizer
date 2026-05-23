using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using PostSource = Wst.Tools.PosiBridge.Application.Source.Post;
using GetListSource = Wst.Tools.PosiBridge.Application.Source.GetList;

namespace Wst.Tools.PosiBridge.Application.Test.Source.GetList;

public class GetListTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostSource.Service _postService = new(new PersistenceStore(fixture.ContextFactory));
    private readonly GetListSource.Service _getListService = new(new PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Get_List_Success()
    {
        var expected1 = NewSource();
        var expected2 = NewSource();

        var postResult1 = await _postService.PostAsync(new PostSource.Command(
            expected1.Name));
        Assert.True(postResult1.IsSucc);
        var posted1 = Assert.IsType<Domain.Source.Source>(postResult1.ToOption().ValueUnsafe());
        Assert.NotNull(posted1);

        var postResult2 = await _postService.PostAsync(new PostSource.Command(expected2.Name));
        Assert.True(postResult2.IsSucc);
        var posted2 = Assert.IsType<Domain.Source.Source>(postResult2.ToOption().ValueUnsafe());
        Assert.NotNull(posted2);

        var getListCommand = new GetListSource.Command(1, 10);
        var actual = await _getListService.GetListAsync(getListCommand);


        Assert.Contains(expected1 with { Id = posted1.Id }, actual);
        Assert.Contains(expected2 with { Id = posted2.Id }, actual);
    }
}
