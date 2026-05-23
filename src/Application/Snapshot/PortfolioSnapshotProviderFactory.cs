using Microsoft.Extensions.DependencyInjection;
using Wst.Tools.PosiBridge.Domain.Snapshot;

namespace Wst.Tools.PosiBridge.Application.Snapshot;

public sealed class PortfolioSnapshotProviderFactory(IServiceProvider serviceProvider)
{
    public IPortfolioSnapshotProvider Get(ESnapshotSource snapshotSource) =>
        serviceProvider.GetRequiredKeyedService<IPortfolioSnapshotProvider>(snapshotSource);
}
