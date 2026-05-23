using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Logging.Abstractions;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.TestCompanion;
using GetPosition = Net.Kidd.Habitizer.Application.Position.Get;
using PutPosition = Net.Kidd.Habitizer.Application.Position.Put;
using SaveSnapshot = Net.Kidd.Habitizer.Application.Snapshot.Save;

namespace Net.Kidd.Habitizer.Application.Test.Snapshot.Save;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly SaveSnapshot.Service _saveService = new(
        new PutPosition.Service(
            new Persistence.Position.PersistenceStore(fixture.ContextFactory),
            new Application.Account.Post.Service(
                new Persistence.Account.PersistenceStore(fixture.ContextFactory),
                new Application.Source.Post.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
            new Application.Instrument.Post.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))),
        NullLogger<SaveSnapshot.Service>.Instance);

    private readonly GetPosition.Service _getPositionService = new(
        new Persistence.Position.PersistenceStore(fixture.ContextFactory),
        new Application.Account.Get.Service(
            new Persistence.Account.PersistenceStore(fixture.ContextFactory),
            new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Application.Instrument.Get.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory)));

    [Fact]
    public async Task Save_SavesAllSnapshotPositionsForAllMappedAccounts()
    {
        var faker = NewFaker();
        var source = faker.PickRandom<ESnapshotSource>();
        var account = NewAccount().WithSource(NewSource().WithName(source.ToSourceName()));
        var expected = NewSnapshot(source, account, 2);

        var maybeSave = await _saveService.SaveAsync(new SaveSnapshot.Command(expected));

        Assert.True(maybeSave.IsSucc);

        foreach (var expectedPosition in expected.Positions)
        {
            var getCommand = new GetPosition.Command(
                expectedPosition.Account.Source.Name,
                expectedPosition.Account.Name,
                expectedPosition.Instrument.Isin);

            var maybePosition = await _getPositionService.GetAsync(getCommand);
            Assert.True(maybePosition.IsSucc);

            var actualPosition = maybePosition.ToOption().ValueUnsafe();
            Assert.NotNull(actualPosition);

            Assert.EquivalentWithExclusions(expectedPosition, actualPosition, 
                x => x.Account.Id,
                x => x.Account.Source.Id,
                x => x.Instrument.Id);
        }
    }
}
