using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;
using SourceId = Net.Kidd.Habitizer.Domain.Source.SourceId;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Source;

[Collection("IntegrationTests")]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task GetPositionsByIdAsync_ReturnsNoneForMissingEntity()
    {
        var missingId = SourceId.New();

        var actual = await _persistenceStore.GetByIdAsync(missingId);

        Assert.True(actual.IsNone);
    }
}


