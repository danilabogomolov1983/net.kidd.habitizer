using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using PositionAddMissing = Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using GetBySourcePosition = Wst.Tools.PosiBridge.Application.Position.Bulk.GetBySource;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.Test.Position.Bulk.GetBySource;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly GetBySourcePosition.Service _service = new(
        new GetByNamesSource.Service(new Persistence.Source.BulkStore(fixture.ContextFactory)),
        new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly PositionAddMissing.Service _addMissingPositionService = new(new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _addMissingSourceService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccountService = new(new Persistence.Account.BulkStore(fixture.ContextFactory));
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = new(new Persistence.Instrument.BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task GetBySourceAsync_ReturnsPositionsFromBulkStore()
    {
        var position1 = NewPosition();
        var position2 = NewPosition();

        Assert.True((await _addMissingSourceService.AddMissingAsync(new SourceAddMissing.Command([position1.Account.Source, position2.Account.Source]))).IsSucc);
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([position1.Account, position2.Account]))).IsSucc);
        Assert.True((await _addMissingInstrumentService.AddMissingAsync(new InstrumentAddMissing.Command([position1.Instrument, position2.Instrument]))).IsSucc);
        Assert.True((await _addMissingPositionService.AddMissingAsync(new PositionAddMissing.Command([position1, position2]))).IsSucc);

        var actual = await _service.GetBySourceAsync(new GetBySourcePosition.Command([position1.Account.Source, position2.Account.Source]));
        Assert.True(actual.IsSucc);
        var result = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, result.Count);
        Assert.Contains(position1, result);
        Assert.Contains(position2, result);
    }

    [Fact]
    public async Task GetBySourceAsync_SourcesDoNotExist()
    {
        var position1 = NewPosition();
        var position2 = NewPosition();

        var actual = await _service.GetBySourceAsync(new GetBySourcePosition.Command([position1.Account.Source, position2.Account.Source]));

        Assert.True(actual.IsFail);
    }
}
