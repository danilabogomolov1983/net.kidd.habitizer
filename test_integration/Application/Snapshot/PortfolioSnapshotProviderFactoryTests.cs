// using FakeItEasy;
// using Microsoft.Extensions.DependencyInjection;
// using Wst.Tools.PosiBridge.Application.Snapshot;
// using Wst.Tools.PosiBridge.Domain.Snapshot;
//
// namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Snapshot;
//
// public class PortfolioSnapshotProviderFactoryTests
// {
//     [Fact]
//     public void Get_ResolvesProviderUsingSnapshotSourceKey()
//     {
//         var expected = A.Fake<IPortfolioSnapshotProvider>();
//         var services = new ServiceCollection();
//         services.AddKeyedScoped<IPortfolioSnapshotProvider>(
//             ESnapshotSource.TickTs,
//             (_, _) => expected);
//
//         using var serviceProvider = services.BuildServiceProvider();
//
//         var factory = new PortfolioSnapshotProviderFactory(serviceProvider);
//
//         var actual = factory.Get(ESnapshotSource.TickTs);
//
//         Assert.Same(expected, actual);
//     }
// }
//
