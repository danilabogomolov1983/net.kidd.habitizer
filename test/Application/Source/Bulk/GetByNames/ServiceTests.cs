using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Persistence.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.Test.Source.Bulk.GetByNames;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly BulkStore _bulkStore = new(fixture.ContextFactory);
    private readonly GetByNamesSource.Service _service = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task GetByNamesAsync_ReturnsSourcesFromBulkStore()
    {
        var source1 = NewSource();
        var source2 = NewSource();

        Assert.True((await _bulkStore.InsertAsync([source1, source2])).IsSucc);

        var actual = await _service.GetByNamesAsync(new GetByNamesSource.Command([source1.Name, source2.Name]));
        Assert.True(actual.IsSucc);
        var actualSources = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualSources.Count);
        Assert.Contains(source1, actualSources);
        Assert.Contains(source2, actualSources);
    }
    
    [Fact]
    public async Task GetByNamesAsync_SourcesDoNotExist()
    {
        var source1 = NewSource();
        var source2 = NewSource();

        var actual = await _service.GetByNamesAsync(new GetByNamesSource.Command([source1.Name, source2.Name]));
        Assert.True(actual.IsFail);
    }
}
