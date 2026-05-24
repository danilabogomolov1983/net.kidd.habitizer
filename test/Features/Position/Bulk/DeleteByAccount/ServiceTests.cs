using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using DeleteByAccountPosition = Net.Kidd.Habitizer.Features.Position.Bulk.DeleteByAccount;
using PositionAddMissing = Net.Kidd.Habitizer.Features.Position.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;
using InstrumentAddMissing = Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using PositionGetByAccounts = Net.Kidd.Habitizer.Features.Position.Bulk.GetByAccounts;
using AccountGetByIds = Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;

namespace Net.Kidd.Habitizer.Features.Test.Position.Bulk.DeleteByAccount;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly DeleteByAccountPosition.Service _service = new(
        new AccountGetByIds.Service(new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly PositionGetByAccounts.Service _getByAccountService = new(
        new AccountGetByIds.Service(new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly PositionAddMissing.Service _addMissingPositionService = new(new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _addMissingSourceService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccountService = new(new Persistence.Account.BulkStore(fixture.ContextFactory));
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = new(new Persistence.Instrument.BulkStore(fixture.ContextFactory));

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
