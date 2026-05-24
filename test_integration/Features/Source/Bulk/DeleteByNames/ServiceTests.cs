using Net.Kidd.Habitizer.TestCompanion;
using DeleteByNamesSource = Net.Kidd.Habitizer.Features.Source.Bulk.DeleteByNames;
using GetByNamesSource = Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Source.Bulk.DeleteByNames;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly SourceAddMissing.Service _addMissingService = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly DeleteByNamesSource.Service _service = fixture.GetRequiredService<DeleteByNamesSource.Service>();
    private readonly GetByNamesSource.Service _getByNamesService = fixture.GetRequiredService<GetByNamesSource.Service>();

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
