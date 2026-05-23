using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;
using DeleteByNamesSource = Net.Kidd.Habitizer.Application.Source.Bulk.DeleteByNames;
using GetByNamesSource = Net.Kidd.Habitizer.Application.Source.Bulk.GetByNames;
using SourceAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Application.Test.Source.Bulk.DeleteByNames;

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
