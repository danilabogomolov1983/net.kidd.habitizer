using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using PostInstrument = Wst.Tools.PosiBridge.Application.Instrument.Post;
using GetInstrument = Wst.Tools.PosiBridge.Application.Instrument.Get;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Instrument.Post;

[Collection("IntegrationTests")]
public class PostInstrumentTests(IntegrationTestsFixture fixture)
{
    private readonly PostInstrument.Service _postService = fixture.GetRequiredService<PostInstrument.Service>();
    private readonly GetInstrument.Service _getService = fixture.GetRequiredService<GetInstrument.Service>();

    [Fact]
    public async Task Post_Get_Success()
    {
        var isin = NewIsin();
        var postCommand = new PostInstrument.Command(isin);

        var maybePosted = await _postService.PostAsync(postCommand);
        Assert.True(maybePosted.IsSucc);
        var expected = maybePosted.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        var getCommand = new GetInstrument.Command(isin);
        var maybeInstrument = await _getService.GetAsync(getCommand);
        Assert.True(maybeInstrument.IsSucc);
        var actual = maybeInstrument.ToOption().ValueUnsafe();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Post_Duplicate_ReturnsExisting()
    {
        var isin = NewIsin();
        var command = new PostInstrument.Command(isin);

        var post = await _postService.PostAsync(command);
        Assert.True(post.IsSucc);

        var postDuplicate = await _postService.PostAsync(command);
        Assert.True(postDuplicate.IsSucc);
        
        var expected = post.ToOption().ValueUnsafe();
        var actual = postDuplicate.ToOption().ValueUnsafe();
        
        Assert.Equal(expected, actual);
    }
}
