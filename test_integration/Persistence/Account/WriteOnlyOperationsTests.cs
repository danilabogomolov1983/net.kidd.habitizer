using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Account;

[Collection("IntegrationTests")]
public class WriteOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    [Fact]
    public async Task Account_CreateAsync()
    {
        var expected = NewAccount();

        await _persistenceStore.CreateAsync(expected);

        var getById = await _persistenceStore.GetByIdAsync(expected.Id);

        Assert.True(getById.IsSome);
        Assert.Equal(expected, getById.ValueUnsafe());
    }

    [Fact]
    public async Task Account_UpdateAsync()
    {
        var original = NewAccount();
        await _persistenceStore.CreateAsync(original);

        var updated = original with { LastUpdatedAt = DateTimeOffset.UtcNow };
        var actual = await _persistenceStore.UpdateAsync(updated);

        Assert.True(actual.IsSucc);
        actual.IfSucc(result => Assert.Equal(updated, result));

        var fetched = await _persistenceStore.GetByIdAsync(original.Id);

        Assert.True(fetched.IsSome);
        Assert.Equal(updated, fetched.ValueUnsafe());
    }
    
    [Fact]
    public async Task Account_SourceExists_CreateAsync()
    {
        var expected1 = NewAccount();

        await _persistenceStore.CreateAsync(expected1);

        var expected2 = NewAccount().WithSource(expected1.Source);
        await _persistenceStore.CreateAsync(expected2);

        var getById = await _persistenceStore.GetByIdAsync(expected2.Id);

        Assert.True(getById.IsSome);
        Assert.Equal(expected2, getById.ValueUnsafe());
    }

    [Fact]
    public async Task Account_CreateAsync_DuplicateNameAndSource_ReturnsFailedFin()
    {
        var expected = NewAccount();

        var created = await _persistenceStore.CreateAsync(expected);
        Assert.True(created.IsSucc);

        var duplicate = expected with { Id = AccountId.New() };

        var createDuplicate = await _persistenceStore.CreateAsync(duplicate);

        Assert.False(createDuplicate.IsSucc);
    }
    
    [Fact]
    public async Task Account_CreateAsync_DuplicateId_ReturnsFailedFin()
    {
        var expected = NewAccount();

        var created = await _persistenceStore.CreateAsync(expected);
        Assert.True(created.IsSucc);

        var duplicate = expected with { Source = NewSource(), Name = NewAccountName() };

        var createDuplicate = await _persistenceStore.CreateAsync(duplicate);

        Assert.False(createDuplicate.IsSucc);
    }
}


