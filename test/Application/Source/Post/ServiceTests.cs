using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using PostSource = Wst.Tools.PosiBridge.Application.Source.Post;
using GetSource = Wst.Tools.PosiBridge.Application.Source.Get;

namespace Wst.Tools.PosiBridge.Application.Test.Source.Post;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostSource.Service _postService = new(new PersistenceStore(fixture.ContextFactory));
    private readonly GetSource.Service _getService = new(new PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Post_Get_Success()
    {
        var sourceName = NewSourceName();
        var postCommand = new PostSource.Command(sourceName);

        var maybePosted = await _postService.PostAsync(postCommand);
        Assert.True(maybePosted.IsSucc);
        var expected = maybePosted.ToOption().ValueUnsafe();
        Assert.NotNull(expected);
        
        var getCommand = new GetSource.Command(sourceName);
        var maybeSource = await _getService.GetAsync(getCommand);
        Assert.True(maybeSource.IsSucc);
        var actual = maybeSource.ToOption().ValueUnsafe();
        
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }
}

