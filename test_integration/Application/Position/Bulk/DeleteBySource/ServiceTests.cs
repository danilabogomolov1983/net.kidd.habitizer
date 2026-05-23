using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using DeleteBySourcePosition = Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteBySource;
using PositionAddMissing = Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using GetBySourcePosition = Wst.Tools.PosiBridge.Application.Position.Bulk.GetBySource;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Position.Bulk.DeleteBySource;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly DeleteBySourcePosition.Service _service = fixture.GetRequiredService<DeleteBySourcePosition.Service>();
    private readonly GetBySourcePosition.Service _getBySourceService = fixture.GetRequiredService<GetBySourcePosition.Service>();
    private readonly PositionAddMissing.Service _addMissingPositionService = fixture.GetRequiredService<PositionAddMissing.Service>();
    private readonly SourceAddMissing.Service _addMissingSourceService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly AccountAddMissing.Service _addMissingAccountService = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = fixture.GetRequiredService<InstrumentAddMissing.Service>();

    [Fact]
    public async Task DeleteAsync_DeletesSourceUsingBulkStore()
    {
        var position1 = NewPosition();
        var position2 = NewPosition();

        Assert.True((await _addMissingSourceService.AddMissingAsync(new SourceAddMissing.Command([position1.Account.Source, position2.Account.Source]))).IsSucc);
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([position1.Account, position2.Account]))).IsSucc);
        Assert.True((await _addMissingInstrumentService.AddMissingAsync(new InstrumentAddMissing.Command([position1.Instrument, position2.Instrument]))).IsSucc);
        Assert.True((await _addMissingPositionService.AddMissingAsync(new PositionAddMissing.Command([position1, position2]))).IsSucc);

        var result = await _service.DeleteBySourcesAsync(new DeleteBySourcePosition.Command([position1.Account.Source]));

        Assert.True(result.IsSucc);

        var deleted = await _getBySourceService.GetBySourceAsync(new GetBySourcePosition.Command([position1.Account.Source]));
        Assert.True(deleted.IsSucc);
        var deletedPositions = deleted.ToOption().ValueUnsafe();

        var remaining = await _getBySourceService.GetBySourceAsync(new GetBySourcePosition.Command([position2.Account.Source]));
        Assert.True(remaining.IsSucc);
        var remainingPositions = remaining.ToOption().ValueUnsafe();

        Assert.Empty(deletedPositions);
        Assert.Single(remainingPositions);
        Assert.Contains(position2, remainingPositions);
    }
}
