using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Persistence.Test.Source;

public class WriteOnlyOperationsTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task Source_CreateAsync()
    {
        var expected = NewSource();

        await _persistenceStore.CreateAsync(expected);

        var getById = await _persistenceStore.GetByIdAsync(expected.Id);

        Assert.True(getById.IsSome);
        Assert.Equal(expected, getById.ValueUnsafe());
    }

    [Fact]
    public async Task Source_CreateAsync_Duplicate_ReturnsFailedFin()
    {
        var expected = NewSource();

        var created = await _persistenceStore.CreateAsync(expected);
        Assert.True(created.IsSucc);

        var duplicate = expected with { Id = SourceId.New() };

        var createDuplicate = await _persistenceStore.CreateAsync(duplicate);

        Assert.False(createDuplicate.IsSucc);
    }
}

