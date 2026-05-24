using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;
using PostSource = Net.Kidd.Habitizer.Features.Source.Post;
using GetSource = Net.Kidd.Habitizer.Features.Source.Get;

namespace Net.Kidd.Habitizer.Features.Test.Source.Post;

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

