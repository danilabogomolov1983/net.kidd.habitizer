using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using DeleteByAccountPosition = Net.Kidd.Habitizer.Application.Position.Bulk.DeleteByAccount;
using PositionAddMissing = Net.Kidd.Habitizer.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using PositionGetByAccounts = Net.Kidd.Habitizer.Application.Position.Bulk.GetByAccounts;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Position.Bulk.DeleteByAccount;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly DeleteByAccountPosition.Service _service = fixture.GetRequiredService<DeleteByAccountPosition.Service>();
    private readonly PositionGetByAccounts.Service _getByAccountService = fixture.GetRequiredService<PositionGetByAccounts.Service>();
    private readonly PositionAddMissing.Service _addMissingPositionService = fixture.GetRequiredService<PositionAddMissing.Service>();
    private readonly SourceAddMissing.Service _addMissingSourceService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly AccountAddMissing.Service _addMissingAccountService = fixture.GetRequiredService<AccountAddMissing.Service>();
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = fixture.GetRequiredService<InstrumentAddMissing.Service>();

    [Fact]
    public async Task DeleteAsync_DeletesAccountsUsingBulkStore()
    {
        var position1 = NewPosition();
        var position2 = NewPosition();

        Assert.True((await _addMissingSourceService.AddMissingAsync(new SourceAddMissing.Command([position1.Account.Source, position2.Account.Source]))).IsSucc);
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([position1.Account, position2.Account]))).IsSucc);
        Assert.True((await _addMissingInstrumentService.AddMissingAsync(new InstrumentAddMissing.Command([position1.Instrument, position2.Instrument]))).IsSucc);
        Assert.True((await _addMissingPositionService.AddMissingAsync(new PositionAddMissing.Command([position1, position2]))).IsSucc);

        var result = await _service.DeleteAsync(new DeleteByAccountPosition.Command([position1.Account]));

        Assert.True(result.IsSucc);

        var deleted = await _getByAccountService.GetByAccountAsync(new PositionGetByAccounts.Command([position1.Account]));
        Assert.True(deleted.IsSucc);
        var deletedPositions = deleted.ToOption().ValueUnsafe();

        var remaining = await _getByAccountService.GetByAccountAsync(new PositionGetByAccounts.Command([position2.Account]));
        Assert.True(remaining.IsSucc);
        var remainingPositions = remaining.ToOption().ValueUnsafe();

        Assert.Empty(deletedPositions);
        Assert.Single(remainingPositions);
        Assert.Contains(position2, remainingPositions);
    }
}
