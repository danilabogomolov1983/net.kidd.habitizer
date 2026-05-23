using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using PositionAddMissing = Net.Kidd.Habitizer.Application.Position.Bulk.AddMissing;
using AccountAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;
using InstrumentAddMissing = Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using PositionGetByAccounts = Net.Kidd.Habitizer.Application.Position.Bulk.GetByAccounts;
using AccountGetByIds = Net.Kidd.Habitizer.Application.Account.Bulk.GetByIds;

namespace Net.Kidd.Habitizer.Application.Test.Position.Bulk.AddMissing;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PositionAddMissing.Service _service = new(new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly PositionGetByAccounts.Service _getByAccountService = new(
        new AccountGetByIds.Service(new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new Persistence.Position.BulkStore(fixture.ContextFactory));
    private readonly SourceAddMissing.Service _addMissingSourceService = new(new Persistence.Source.BulkStore(fixture.ContextFactory));
    private readonly AccountAddMissing.Service _addMissingAccountService = new(new Persistence.Account.BulkStore(fixture.ContextFactory));
    private readonly InstrumentAddMissing.Service _addMissingInstrumentService = new(new Persistence.Instrument.BulkStore(fixture.ContextFactory));

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

    [Fact]
    public async Task AddMissingAsync_SameInstrumentForDifferentAccounts_AddsBothPositions()
    {
        var sharedInstrument = NewInstrument();
        var position1 = NewPosition() with { Instrument = sharedInstrument };
        var position2 = NewPosition() with { Instrument = sharedInstrument };

        Assert.True((await _addMissingSourceService.AddMissingAsync(new SourceAddMissing.Command([position1.Account.Source, position2.Account.Source]))).IsSucc);
        Assert.True((await _addMissingAccountService.AddMissingAsync(new AccountAddMissing.Command([position1.Account, position2.Account]))).IsSucc);
        Assert.True((await _addMissingInstrumentService.AddMissingAsync(new InstrumentAddMissing.Command([sharedInstrument]))).IsSucc);

        var result = await _service.AddMissingAsync(new PositionAddMissing.Command([position1, position2]));
        Assert.True(result.IsSucc);

        var actual = await _getByAccountService.GetByAccountAsync(new PositionGetByAccounts.Command([position1.Account, position2.Account]));
        Assert.True(actual.IsSucc);
        var positions = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, positions.Count);
        Assert.Contains(position1, positions);
        Assert.Contains(position2, positions);
    }
}
