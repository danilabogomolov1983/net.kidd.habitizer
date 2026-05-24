using LanguageExt;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Snapshot.Sync;

public class Service(
    PortfolioSnapshotProviderFactory portfolioSnapshotProviderFactory,
    Snapshot.Merge.Service snapshotMergeService)
{
    public Task<Fin<Unit>> SyncAsync(ESnapshotSource snapshotSource, string[] accounts)
    {
        var provider = portfolioSnapshotProviderFactory.Get(snapshotSource);

        return provider.GetAsync(accounts)
            .MapAsync(snapshot => new Merge.Command(snapshot))
            .BindAsync(snapshotMergeService.MergeAsync);
    }
}
