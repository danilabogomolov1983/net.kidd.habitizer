using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Snapshot.Sync;

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
