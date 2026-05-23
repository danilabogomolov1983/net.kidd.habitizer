using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;
using SourceId = Net.Kidd.Habitizer.Domain.Source.SourceId;

namespace Net.Kidd.Habitizer.Persistence.Test.Source;

public class ReadOnlyOperationsTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task GetPositionsByIdAsync_ReturnsNoneForMissingEntity()
    {
        var missingId = SourceId.New();

        var actual = await _persistenceStore.GetByIdAsync(missingId);

        Assert.True(actual.IsNone);
    }

    [Fact]
    public async Task GetPositionsAsync_UsesPaging()
    {
        const int expectedPageSize = 10;

        var expectedItems = Enumerable
            .Range(0, 25)
            .Select(_ => NewSource())
            .ToList();

        foreach (var expectedItem in expectedItems)
        {
            await _persistenceStore.CreateAsync(expectedItem);
        }

        var page1 = await _persistenceStore.GetListAsync(pageNumber: 1, pageSize: expectedPageSize);
        var page2 = await _persistenceStore.GetListAsync(pageNumber: 2, pageSize: expectedPageSize);
        var page3 = await _persistenceStore.GetListAsync(pageNumber: 3, pageSize: expectedPageSize);

        Assert.Equal(expectedPageSize, page1.Count);
        Assert.Equal(expectedPageSize, page2.Count);
        Assert.Equal(5, page3.Count);

        Assert.All(page1, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page2, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page3, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
    }
}

