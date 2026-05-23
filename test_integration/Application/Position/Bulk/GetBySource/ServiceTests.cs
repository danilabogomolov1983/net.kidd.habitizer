using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using PositionAddMissing = Net.Kidd.Habitizer.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using GetBySourcePosition = Net.Kidd.Habitizer.Application.Position.Bulk.GetBySource;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Position.Bulk.GetBySource;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetBySourcePosition.Service _service = fixture.GetRequiredService<GetBySourcePosition.Service>();
    private readonly PositionAddMissing.Service _addMissingPositionService = fixture.GetRequiredService<PositionAddMissing.Service>();
    private readonly SourceAddMissing.Service _addMissingSourceService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly AccountAddMissing.Service _addMissingAccountService = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = fixture.GetRequiredService<InstrumentAddMissing.Service>();

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
