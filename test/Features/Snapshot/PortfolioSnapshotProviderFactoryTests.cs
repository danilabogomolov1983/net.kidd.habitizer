using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Snapshot;

namespace Net.Kidd.Habitizer.Features.Test.Snapshot;

public class PortfolioSnapshotProviderFactoryTests
{
    [Fact]
    public void Get_ResolvesProviderUsingSnapshotSourceKey()
    {
        var expected = A.Fake<IPortfolioSnapshotProvider>();
        var services = new ServiceCollection();
        services.AddKeyedScoped<IPortfolioSnapshotProvider>(
            ESnapshotSource.TickTs,
            (_, _) => expected);

        using var serviceProvider = services.BuildServiceProvider();

        var factory = new PortfolioSnapshotProviderFactory(serviceProvider);

        var actual = factory.Get(ESnapshotSource.TickTs);

        Assert.Same(expected, actual);
    }
}
