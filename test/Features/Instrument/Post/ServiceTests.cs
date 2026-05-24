using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.TestCompanion;
using PostInstrument = Net.Kidd.Habitizer.Features.Instrument.Post;
using GetInstrument = Net.Kidd.Habitizer.Features.Instrument.Get;

namespace Net.Kidd.Habitizer.Features.Test.Instrument.Post;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostInstrument.Service _postService = new(new PersistenceStore(fixture.ContextFactory));
    private readonly GetInstrument.Service _getService = new(new PersistenceStore(fixture.ContextFactory));

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
}

