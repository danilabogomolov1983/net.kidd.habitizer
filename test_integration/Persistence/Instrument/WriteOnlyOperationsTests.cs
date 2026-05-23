using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Domain.Instrument;
using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.TestCompanion;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Instrument;

[Collection("IntegrationTests")]
public class WriteOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task Instrument_CreateAsync()
    {
        var expected = NewInstrument();

        await _persistenceStore.CreateAsync(expected);

        var getById = await _persistenceStore.GetByIdAsync(expected.Id);

        Assert.True(getById.IsSome);
        Assert.Equal(expected, getById.ValueUnsafe());
    }

    [Fact]
    public async Task Instrument_CreateAsync_Duplicate_ReturnsFailedFin()
    {
        var expected = NewInstrument();

        var created = await _persistenceStore.CreateAsync(expected);
        Assert.True(created.IsSucc);

        var createDuplicate = await _persistenceStore.CreateAsync(expected);

        Assert.False(createDuplicate.IsSucc);
    }
}


