using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using PositionAddMissing = Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using PositionGetByAccounts = Wst.Tools.PosiBridge.Application.Position.Bulk.GetByAccounts;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Position.Bulk.AddMissing;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly PositionAddMissing.Service _service = fixture.GetRequiredService<PositionAddMissing.Service>();
    private readonly PositionGetByAccounts.Service _getByAccountService = fixture.GetRequiredService<PositionGetByAccounts.Service>();
    private readonly SourceAddMissing.Service _addMissingSourceService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly AccountAddMissing.Service _addMissingAccountService = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = fixture.GetRequiredService<InstrumentAddMissing.Service>();

    [Fact]
    public async Task AddMissingAsync_ExistingPositionsExcludedFromInsert()
    {
        var existingPosition = NewPosition();
        var missingPosition = NewPosition();

        Assert.True((await _addMissingSourceService.AddMissingAsync(new SourceAddMissing.Command([existingPosition.Account.Source, missingPosition.Account.Source]))).IsSucc);
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([existingPosition.Account, missingPosition.Account]))).IsSucc);
        Assert.True((await _addMissingInstrumentService.AddMissingAsync(new InstrumentAddMissing.Command([existingPosition.Instrument, missingPosition.Instrument]))).IsSucc);
        
        Assert.True((await _service.AddMissingAsync(new PositionAddMissing.Command([existingPosition]))).IsSucc);

        Assert.True((await _service.AddMissingAsync(new PositionAddMissing.Command([existingPosition, missingPosition]))).IsSucc);


        var actual = await _getByAccountService.GetByAccountAsync(new PositionGetByAccounts.Command([existingPosition.Account, missingPosition.Account]));
        Assert.True(actual.IsSucc);
        var positions = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, positions.Count);
        Assert.Contains(existingPosition, positions);
        Assert.Contains(missingPosition, positions);
    }
}
