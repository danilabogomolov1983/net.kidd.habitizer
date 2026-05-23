using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Persistence.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using SourceId = Wst.Tools.PosiBridge.Domain.Source.SourceId;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Source;

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


