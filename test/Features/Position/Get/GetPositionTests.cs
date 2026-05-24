using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.TestCompanion;
using PostPosition = Net.Kidd.Habitizer.Features.Position.Post;
using GetPosition = Net.Kidd.Habitizer.Features.Position.Get;

namespace Net.Kidd.Habitizer.Features.Test.Position.Get;

public class GetPositionTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostPosition.Service _postService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Features.Account.Post.Service(new Persistence.Account.PersistenceStore(fixture.ContextFactory), new Features.Source.Post.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Features.Instrument.Post.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))
    );
    private readonly GetPosition.Service _getService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Features.Account.Get.Service(new Persistence.Account.PersistenceStore(fixture.ContextFactory), new Features.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Features.Instrument.Get.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))
    );

    [Fact]
    public async Task Get_Success()
    {
        var position = NewPosition();
        var postCommand = new PostPosition.Command(
            position.Account.Source.Name,
            position.Account.Name,
            position.Instrument.Isin,
            position.NetSize,
            position.NetValue,
            position.UnrealisedAverageCost,
            position.UnrealisedProfit,
            position.UnrealisedProfitPercent,
            position.ReferencePrice);

        var maybePosted = await _postService.PostAsync(postCommand);
        Assert.True(maybePosted.IsSucc);
        var expected = maybePosted.ToOption().ValueUnsafe();
        
        var getCommand = new GetPosition.Command(expected.Account.Source.Name, expected.Account.Name, expected.Instrument.Isin);
        var maybePosition = await _getService.GetAsync(getCommand);
        Assert.True(maybePosition.IsSucc);
        var actual = maybePosition.ToOption().ValueUnsafe();
        
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);

    }
}
