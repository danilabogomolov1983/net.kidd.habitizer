using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.TestCompanion;

using MergeSnapshot = Net.Kidd.Habitizer.Features.Snapshot.Merge;
using GetPosition = Net.Kidd.Habitizer.Features.Position.Get;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Snapshot.Merge;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private static DateTimeOffset Now => new(2026, 5, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly MergeSnapshot.Service _mergeService = new(
        new(new Persistence.Source.BulkStore(fixture.ContextFactory)),
        new(new Persistence.Instrument.BulkStore(fixture.ContextFactory)),
        new(new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new(new(new Persistence.Source.PersistenceStore(fixture.ContextFactory)),
            new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new(new Persistence.Account.BulkStore(fixture.ContextFactory)),
        new(new Persistence.Position.BulkStore(fixture.ContextFactory)),
        new(new(new Persistence.Source.BulkStore(fixture.ContextFactory)), new Persistence.Position.BulkStore(fixture.ContextFactory)),
        new FakeTimeProvider(Now),
        NullLogger<MergeSnapshot.Service>.Instance);

    private readonly GetPosition.Service _getPositionService = fixture.GetRequiredService<GetPosition.Service>();

    [Fact]
    public async Task Merge_DeletesAllsSnapshotPositionsBySourceAndAddsNewPositions()
    {
        var faker = NewFaker();
        var source = faker.PickRandom<ESnapshotSource>();
        var account = NewAccount().WithSource(NewSource().WithName(source.ToSourceName()));
        var expected = NewSnapshot(source, account, 2);

        var maybeSave = await _mergeService.MergeAsync(new MergeSnapshot.Command(expected));

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
                x => x.Account.LastUpdatedAt,
                x => x.Instrument.Id);
            
            Assert.EquivalentWithExclusions(expectedPosition.Account.WithLastUpdatedAt(Now), actualPosition.Account,
                x => x.Source.Id);

        }
    }
}
