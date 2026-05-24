using Microsoft.Extensions.DependencyInjection;
using Net.Kidd.Habitizer.Domain.Snapshot;

namespace Net.Kidd.Habitizer.Features.Snapshot;

public sealed class PortfolioSnapshotProviderFactory(IServiceProvider serviceProvider)
{
    public IPortfolioSnapshotProvider Get(ESnapshotSource snapshotSource) =>
        serviceProvider.GetRequiredKeyedService<IPortfolioSnapshotProvider>(snapshotSource);
}
