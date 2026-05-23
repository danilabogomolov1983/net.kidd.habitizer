using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.TestCompanion;
using PostInstrument = Wst.Tools.PosiBridge.Application.Instrument.Post;
using GetListInstrument = Wst.Tools.PosiBridge.Application.Instrument.GetList;

namespace Wst.Tools.PosiBridge.Application.Test.Instrument.GetList;

public class GetListTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PostInstrument.Service _postService = new(new PersistenceStore(fixture.ContextFactory));
    private readonly GetListInstrument.Service _getListService = new(new PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Get_List_Success()
    {
        var expected1 = NewInstrument();
        var expected2 = NewInstrument();

        var postResult1 = await _postService.PostAsync(new PostInstrument.Command(
            expected1.Isin.Value));
        Assert.True(postResult1.IsSucc);
        var posted1 = Assert.IsType<Domain.Instrument.Instrument>(postResult1.ToOption().ValueUnsafe());
        Assert.NotNull(posted1);

        var postResult2 = await _postService.PostAsync(new PostInstrument.Command(expected2.Isin.Value));
        Assert.True(postResult2.IsSucc);
        var posted2 = Assert.IsType<Domain.Instrument.Instrument>(postResult2.ToOption().ValueUnsafe());
        Assert.NotNull(posted2);

        var getListCommand = new GetListInstrument.Command(1, 10);
        var actual = await _getListService.GetListAsync(getListCommand);


        Assert.Contains(expected1 with { Id = posted1.Id }, actual);
        Assert.Contains(expected2 with { Id = posted2.Id }, actual);
    }
}
