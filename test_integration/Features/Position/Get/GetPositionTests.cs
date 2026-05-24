using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using PostPosition = Net.Kidd.Habitizer.Features.Position.Post;
using GetPosition = Net.Kidd.Habitizer.Features.Position.Get;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Position.Get;

[Collection("IntegrationTests")]
public class GetPositionTests(IntegrationTestsFixture fixture)
{
    private readonly PostPosition.Service _postService = fixture.GetRequiredService<PostPosition.Service>();
    private readonly GetPosition.Service _getService = fixture.GetRequiredService<GetPosition.Service>();

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
