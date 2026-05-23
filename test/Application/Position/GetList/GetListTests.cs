using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Position;
using Wst.Tools.PosiBridge.TestCompanion;
using PostPosition = Wst.Tools.PosiBridge.Application.Position.Post;
using GetListPosition = Wst.Tools.PosiBridge.Application.Position.GetList;

namespace Wst.Tools.PosiBridge.Application.Test.Position.GetList;

public class GetListTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostPosition.Service _postService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Application.Account.Post.Service(new Persistence.Account.PersistenceStore(fixture.ContextFactory), new Application.Source.Post.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Application.Instrument.Post.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))
    );

    private readonly GetListPosition.Service _getListService = new(new PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Get_List_Success()
    {
        var expected1 = NewPosition();
        var expected2 = NewPosition();

        var postResult1 = await _postService.PostAsync(new PostPosition.Command(
            expected1.Account.Source.Name,
            expected1.Account.Name,
            expected1.Instrument.Isin,
            expected1.NetSize,
            expected1.NetValue,
            expected1.UnrealisedAverageCost,
            expected1.UnrealisedProfit,
            expected1.UnrealisedProfitPercent,
            expected1.ReferencePrice));
        Assert.True(postResult1.IsSucc);
        var posted1 = Assert.IsType<Domain.Position.Position>(postResult1.ToOption().ValueUnsafe());
        Assert.NotNull(posted1);

        var postResult2 = await _postService.PostAsync(new PostPosition.Command(
            expected2.Account.Source.Name,
            expected2.Account.Name,
            expected2.Instrument.Isin,
            expected2.NetSize,
            expected2.NetValue,
            expected2.UnrealisedAverageCost,
            expected2.UnrealisedProfit,
            expected2.UnrealisedProfitPercent,
            expected2.ReferencePrice));
        Assert.True(postResult2.IsSucc);
        var posted2 = Assert.IsType<Domain.Position.Position>(postResult2.ToOption().ValueUnsafe());
        Assert.NotNull(posted2);

        var getListCommand = new GetListPosition.Command(1, 10);
        var actual = await _getListService.GetListAsync(getListCommand);


        Assert.Contains(actual, item =>
            item.Account.Source.Name == expected1.Account.Source.Name &&
            item.Account.Name == expected1.Account.Name &&
            item.Instrument.Isin == expected1.Instrument.Isin &&
            item.NetSize == expected1.NetSize &&
            item.NetValue == expected1.NetValue &&
            item.UnrealisedAverageCost == expected1.UnrealisedAverageCost &&
            item.UnrealisedProfit == expected1.UnrealisedProfit &&
            item.UnrealisedProfitPercent == expected1.UnrealisedProfitPercent &&
            item.ReferencePrice == expected1.ReferencePrice);

        Assert.Contains(actual, item =>
            item.Account.Source.Name == expected2.Account.Source.Name &&
            item.Account.Name == expected2.Account.Name &&
            item.Instrument.Isin == expected2.Instrument.Isin &&
            item.NetSize == expected2.NetSize &&
            item.NetValue == expected2.NetValue &&
            item.UnrealisedAverageCost == expected2.UnrealisedAverageCost &&
            item.UnrealisedProfit == expected2.UnrealisedProfit &&
            item.UnrealisedProfitPercent == expected2.UnrealisedProfitPercent &&
            item.ReferencePrice == expected2.ReferencePrice);
    }
}
