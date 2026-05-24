using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using PostSource = Net.Kidd.Habitizer.Features.Source.Post;
using GetSource = Net.Kidd.Habitizer.Features.Source.Get;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Source.Post;

[Collection("IntegrationTests")]
public class PostSourceTests(IntegrationTestsFixture fixture)
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

    [Fact]
    public async Task Post_Duplicate_ReturnsExisting()
    {
        var sourceName = NewSourceName();
        var command = new PostSource.Command(sourceName);

        var post = await _postService.PostAsync(command);
        Assert.True(post.IsSucc);

        var postDuplicate = await _postService.PostAsync(command);
        Assert.True(postDuplicate.IsSucc);
        
        var expected = post.ToOption().ValueUnsafe();
        var actual = postDuplicate.ToOption().ValueUnsafe();
        
        Assert.Equal(expected, actual);
    }
}
