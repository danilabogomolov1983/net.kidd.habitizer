using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Persistence.IntegrationTest.Position;

[Collection("IntegrationTests")]
public class WriteOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task Position_CreateAsync()
    {
        var expected = NewPosition();

        await _persistenceStore.CreateAsync(expected);

        var getById = await _persistenceStore.GetByIdAsync(expected.Account.Id, expected.Instrument.Id);

        Assert.True(getById.IsSome);
        Assert.Equal(expected, getById.ValueUnsafe());
    }

    [Fact]
    public async Task Position_UpdateAsync()
    {
        var original = NewPosition();
        await _persistenceStore.CreateAsync(original);

        var updated = original with { NetValue = NewDecimal() };
        var actual = await _persistenceStore.UpdateAsync(updated);

        Assert.True(actual.IsSucc);
        actual.IfSucc(result => Assert.Equal(updated, result));

        var fetched = await _persistenceStore.GetByIdAsync(original.Account.Id, original.Instrument.Id);

        Assert.True(fetched.IsSome);
        Assert.Equal(updated, fetched.ValueUnsafe());
    }

    [Fact]
    public async Task Position_CreateAsync_Duplicate_ReturnsFailedFin()
    {
        var expected = NewPosition();

        var created = await _persistenceStore.CreateAsync(expected);
        Assert.True(created.IsSucc);

        var createDuplicate = await _persistenceStore.CreateAsync(expected);

        Assert.False(createDuplicate.IsSucc);
    }
}


