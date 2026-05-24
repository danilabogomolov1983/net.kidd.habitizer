using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;

namespace Net.Kidd.Habitizer.Tradix.IntegrationTest.Snapshot;

public class SnapshotProviderTests(TradixFixture fixture) : IClassFixture<TradixFixture>
{
    private static readonly Source TradixSource = new(SourceId.Empty(), ESnapshotSource.Tradix.ToSourceName());
    private readonly IPortfolioSnapshotProvider _provider = fixture.SnapshotProvider;

    private async Task<List<Domain.Position.Position>> SeedPositions(string expectedAccount, bool deleteOldData = false)
    {
        await using var dbContext = await fixture.SnapshotDbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);

        if (deleteOldData)
        {
            await Support.Infrastructure.DeletePositionsAsync(dbContext);
        }
        var seededPositions = Support.Tradix.SeedPositions(quantity: 10, expectedAccount)
            .Append(Support.Tradix.SeedNullPositions(quantity:5, expectedAccount))
            .Append(Support.Tradix.SeedInvalidPositions(quantity:3, expectedAccount))
            .ToList();

        seededPositions.Iter(position =>
            Support.Infrastructure.InsertPositionAsync(dbContext, position).GetAwaiter().GetResult());
        return seededPositions.Map(i => TradixMappers.SnapshotPositionMapper.Map((TradixSource, i))).ToList();
        
    }
    
    [Fact]
    public async Task GetAsync_ReturnsOnlyRequestedAccount()
    {
        var faker = NewFaker();
        var expectedAccount = faker.PickRandom<string>(Support.Tradix.Depot1, Support.Tradix.Depot2);

        var seededPositions = await SeedPositions(expectedAccount, true);
        var expectedPositions = seededPositions
            .Where(i => i.Account.Name == expectedAccount)
            .ToList();

        var maybeSnapshot = await _provider.GetAsync(expectedAccount);

        var actual = maybeSnapshot.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        
        Assert.All(expectedPositions, item => Assert.Contains(item, actual.Positions));
    }

    [Fact]
    public async Task GetAsync_ReturnsEmpty_WhenAccountDoesNotExist()
    {
        var faker = NewFaker();
        var expectedAccount = faker.PickRandom<string>(Support.Tradix.Depot1, Support.Tradix.Depot2);
        await SeedPositions(expectedAccount);
        
        var maybeSnapshot = await _provider.GetAsync("UNKNOWN_DEPOT");
        var actual = maybeSnapshot.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Positions);
    }

    [Fact]
    public async Task GetAsync_ReturnsOnlyRequestedAccounts()
    {
        var account1 = Support.Tradix.Depot1;
        var account2 = Support.Tradix.Depot2;
        var excludedAccount = "DEPOT3";

        var seededPositions1 = await SeedPositions(account1, true);
        var seededPositions2 = await SeedPositions(account2);
        var seededPositions3 = await SeedPositions(excludedAccount);
        var expectedPositions1 = seededPositions1
            .Where(i => i.Account.Name == account1)
            .ToList();
        var expectedPositions2 = seededPositions2
            .Where(i => i.Account.Name == account2)
            .ToList();

        var maybeSnapshot = await _provider.GetAsync([account1, account2]);

        var actual = maybeSnapshot.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        
        Assert.All(expectedPositions1, item => Assert.Contains(item, actual.Positions));
        Assert.All(expectedPositions2, item => Assert.Contains(item, actual.Positions));
    }
}
