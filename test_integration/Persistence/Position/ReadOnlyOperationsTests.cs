using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Position;

[Collection("IntegrationTests")]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task GetPositionsByIdAsync_ReturnsNoneForMissingEntity()
    {
        var actual = await _persistenceStore.GetByIdAsync(
            new Domain.Account.AccountId(Guid.NewGuid()),
            new Domain.Instrument.InstrumentId(Guid.NewGuid()));

        Assert.True(actual.IsNone);
    }
}


