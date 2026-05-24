using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.TestCompanion;
using GetByNamesSource = Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;

namespace Net.Kidd.Habitizer.Features.IntegrationTest.Source.Bulk.GetByNames;


[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly Domain.Source.IBulkStore _bulkStore = fixture.GetRequiredService<Domain.Source.IBulkStore>();
    private readonly GetByNamesSource.Service _service = fixture.GetRequiredService<GetByNamesSource.Service>();

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
