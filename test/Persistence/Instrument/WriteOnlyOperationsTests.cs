using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Persistence.Test.Instrument;

public class WriteOnlyOperationsTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
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

