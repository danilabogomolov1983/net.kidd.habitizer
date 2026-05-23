using FakeItEasy;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Wst.Tools.PosiBridge.Application.Snapshot;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using GetPosition = Wst.Tools.PosiBridge.Application.Position.Get;
using MergeSnapshot = Wst.Tools.PosiBridge.Application.Snapshot.Merge;
using SyncSnapshot = Wst.Tools.PosiBridge.Application.Snapshot.Sync;

namespace Wst.Tools.PosiBridge.Application.Test.Snapshot.Sync;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
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

    private readonly GetPosition.Service _getPositionService = new(
        new Persistence.Position.PersistenceStore(fixture.ContextFactory),
        new Application.Account.Get.Service(
            new Persistence.Account.PersistenceStore(fixture.ContextFactory),
            new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Application.Instrument.Get.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory)));

    [Theory]
    [InlineData(ESnapshotSource.TickTs)]
    [InlineData(ESnapshotSource.Tradix)]
    public async Task SyncAsync_MergesSnapshotsForConfiguredSourceAccounts(ESnapshotSource source)
    {
        var account = NewAccount().WithSource(NewSource().WithName(source.ToSourceName()));
        var expected = NewSnapshot(source, account, 2);

        var provider = A.Fake<IPortfolioSnapshotProvider>();
        A.CallTo(() => provider.GetAsync(A<string[]>._)).Returns(expected);

        var service = NewService(source, provider);

        var result = await service.SyncAsync(source, [account.Name]);
        Assert.True(result.IsSucc);
        
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
    
    private SyncSnapshot.Service NewService(ESnapshotSource source, IPortfolioSnapshotProvider provider)
    {
        var services = new ServiceCollection();
        services.AddKeyedScoped<IPortfolioSnapshotProvider>(source, (_, _) => provider);
        var serviceProvider = services.BuildServiceProvider();

        return new SyncSnapshot.Service(new PortfolioSnapshotProviderFactory(serviceProvider), _mergeService);
    }
}
