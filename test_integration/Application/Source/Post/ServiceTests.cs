using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using PostSource = Wst.Tools.PosiBridge.Application.Source.Post;
using GetSource = Wst.Tools.PosiBridge.Application.Source.Get;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Source.Post;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly PostSource.Service _postService = fixture.GetRequiredService<PostSource.Service>();
    private readonly GetSource.Service _getService = fixture.GetRequiredService<GetSource.Service>();

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

