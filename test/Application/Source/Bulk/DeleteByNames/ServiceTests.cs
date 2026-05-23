using Wst.Tools.PosiBridge.Persistence.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using DeleteByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.DeleteByNames;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;

namespace Wst.Tools.PosiBridge.Application.Test.Source.Bulk.DeleteByNames;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly SourceAddMissing.Service _addMissingService = new(new BulkStore(fixture.ContextFactory));
    private readonly DeleteByNamesSource.Service _service = new(new GetByNamesSource.Service(new BulkStore(fixture.ContextFactory)), new BulkStore(fixture.ContextFactory));
    private readonly GetByNamesSource.Service _getByNamesService = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task DeleteAsync_DeletesNamesUsingBulkStore()
    {
        var source1 = NewSource();
        var source2 = NewSource();

        Assert.True((await _addMissingService.AddMissingAsync(new SourceAddMissing.Command([source1, source2]))).IsSucc);

        var result = await _service.DeleteAsync(new DeleteByNamesSource.Command([source1.Name, source2.Name]));

        Assert.True(result.IsSucc);

        var actual = await _getByNamesService.GetByNamesAsync(new GetByNamesSource.Command([source1.Name, source2.Name]));
        Assert.Empty(actual);
    }
    
    [Fact]
    public async Task DeleteAsync_SourcesDoNotExist()
    {
        var source1 = NewSource();
        var source2 = NewSource();
     
        var result = await _service.DeleteAsync(new DeleteByNamesSource.Command([source1.Name, source2.Name]));

        Assert.False(result.IsSucc);

        var actual = await _getByNamesService.GetByNamesAsync(new GetByNamesSource.Command([source1.Name, source2.Name]));
        Assert.False(actual.IsSucc);
    }
}
