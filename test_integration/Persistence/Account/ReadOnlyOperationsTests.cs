using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.TestCompanion;
using AccountId = Wst.Tools.PosiBridge.Domain.Account.AccountId;

namespace Wst.Tools.PosiBridge.Persistence.IntegrationTest.Account;

[Collection("IntegrationTests")]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IPersistenceStore _persistenceStore = new PersistenceStore(fixture.ContextFactory);

    private readonly IBulkStore _bulkStore =
        new BulkStore(fixture.ContextFactory);

    [Fact]
    public async Task GetAccountsByIdAsync_ReturnsNoneForMissingEntity()
    {
        var missingId = AccountId.New();

        var actual = await _persistenceStore.GetByIdAsync(missingId);

        Assert.True(actual.IsNone);
    }

    [Fact]
    public async Task GetAccountsAsync_UsesPaging()
    {
        const int expectedPageSize = 10;

        var existingItems = await _persistenceStore.GetListAsync(pageNumber: 1, pageSize: int.MaxValue);
        var deleteExisting = await _bulkStore.DeleteAsync(existingItems.Distinct().ToList());
        Assert.True(deleteExisting.IsSucc);

        var expectedItems = Enumerable
            .Range(0, 25)
            .Select(_ => NewAccount())
            .ToList();

        foreach (var expectedItem in expectedItems)
        {
            await _persistenceStore.CreateAsync(expectedItem);
        }

        var page1 = await _persistenceStore.GetListAsync(pageNumber: 1, pageSize: expectedPageSize);
        var page2 = await _persistenceStore.GetListAsync(pageNumber: 2, pageSize: expectedPageSize);
        var page3 = await _persistenceStore.GetListAsync(pageNumber: 3, pageSize: expectedPageSize);

        Assert.Equal(expectedPageSize, page1.Count);
        Assert.Equal(expectedPageSize, page2.Count);
        Assert.Equal(5, page3.Count);

        Assert.All(page1, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page2, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page3, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
    }
    
    [Fact]
    public async Task GetAccountsBySourceAsync_UsesPaging()
    {
        const int expectedPageSize = 10;

        var source = NewSource();
        var expectedItems = Enumerable
            .Range(0, 25)
            .Select(_ => NewAccount() with{ Source = source})
            .ToList();

        foreach (var expectedItem in expectedItems)
        {
            await _persistenceStore.CreateAsync(expectedItem);
        }

        var page1 = await _persistenceStore.GetBySourceAsync(source.Id, pageNumber: 1, pageSize: expectedPageSize);
        var page2 = await _persistenceStore.GetBySourceAsync(source.Id, pageNumber: 2, pageSize: expectedPageSize);
        var page3 = await _persistenceStore.GetBySourceAsync(source.Id, pageNumber: 3, pageSize: expectedPageSize);

        Assert.Equal(expectedPageSize, page1.Count);
        Assert.Equal(expectedPageSize, page2.Count);
        Assert.Equal(5, page3.Count);

        Assert.All(page1, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page2, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
        Assert.All(page3, item => Assert.Contains(expectedItems, expected => expected.Id == item.Id));
    }
}


