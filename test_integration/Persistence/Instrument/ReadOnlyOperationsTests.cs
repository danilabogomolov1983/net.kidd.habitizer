using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.TestCompanion;
using InstrumentId = Net.Kidd.Habitizer.Domain.Instrument.InstrumentId;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Instrument;

[Collection("IntegrationTests")]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task GetPositionsByIdAsync_ReturnsNoneForMissingEntity()
    {
        var missingId = InstrumentId.New();

        var actual = await _persistenceStore.GetByIdAsync(missingId);

        Assert.True(actual.IsNone);
    }
}


