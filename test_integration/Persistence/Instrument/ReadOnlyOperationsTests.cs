using Wst.Tools.PosiBridge.Domain.Instrument;
using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.TestCompanion;
using InstrumentId = Wst.Tools.PosiBridge.Domain.Instrument.InstrumentId;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Instrument;

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


