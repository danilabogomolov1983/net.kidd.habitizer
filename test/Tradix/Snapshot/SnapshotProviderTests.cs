using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Tradix.Snapshot;

namespace Net.Kidd.Habitizer.Tradix.Test.Snapshot;

public class SnapshotProviderTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private static readonly Source TradixSource = new(SourceId.Empty(), ESnapshotSource.Tradix.ToSourceName());

    private readonly SnapshotProvider _provider = new(fixture.ContextFactory);

    [Fact]
    public async Task GetAsync_ReturnsOnlyRequestedAccount()
    {
        var faker = NewFaker();
        var expectedAccount = faker.PickRandom<string>(Support.Tradix.Depot1, Support.Tradix.Depot2);

        var seededPositions = await SeedPositions(expectedAccount);
        var expectedPositions = seededPositions
            .Where(i => i.Account.Name == expectedAccount)
            .ToList();

        var maybeSnapshot = await _provider.GetAsync(expectedAccount);

        var actual = maybeSnapshot.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        
        Assert.All(expectedPositions, item => Assert.Contains(item, actual.Positions));
    }

    [Fact]
    public async Task GetAsync_ReturnsOnlyRequestedAccounts()
    {
        var account1 = Support.Tradix.Depot1;
        var account2 = Support.Tradix.Depot2;
        var excludedAccount = "DEPOT3";

        var seededPositions1 = await SeedPositions(account1);
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

    private async Task<List<Position>> SeedPositions(string expectedAccount)
    {
        await using var dbContext = await fixture.ContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);

        var seededPositions = Support.Tradix.SeedPositions(quantity: 10, expectedAccount)
            .Append(Support.Tradix.SeedNullPositions(quantity:5, expectedAccount))
            .Append(Support.Tradix.SeedInvalidPositions(quantity:3, expectedAccount))
            .ToList();

        seededPositions.Iter(position =>
            Support.Infrastructure.InsertPositionAsync(dbContext, position).GetAwaiter().GetResult());
        return seededPositions.Map(i => TradixMappers.SnapshotPositionMapper.Map((TradixSource, i))).ToList();
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
}
