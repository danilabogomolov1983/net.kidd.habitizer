using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.TestCompanion;
using SourceAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using GetByNamesSource = Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;

namespace Net.Kidd.Habitizer.Features.Test.Source.Bulk.AddMissing;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly SourceAddMissing.Service _service = new(new BulkStore(fixture.ContextFactory));
    private readonly GetByNamesSource.Service _getByNamesService = new(new BulkStore(fixture.ContextFactory));

    [Fact]
    public async Task AddMissingAsync_ExistingSourcesExcludedFromInsert()
    {
        var existingSource = NewSource();
        var missingSource = NewSource();

        Assert.True((await _service.AddMissingAsync(new SourceAddMissing.Command([existingSource]))).IsSucc);
        
        Assert.False((await _getByNamesService.GetByNamesAsync(new GetByNamesSource.Command([existingSource.Name, missingSource.Name]))).IsSucc);
        
        var result = await _service.AddMissingAsync(new SourceAddMissing.Command([existingSource, missingSource]));
        Assert.True(result.IsSucc);

        var actual = await _getByNamesService.GetByNamesAsync(new GetByNamesSource.Command([existingSource.Name, missingSource.Name]));
        Assert.True(actual.IsSucc);
        var actualSources = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualSources.Count);
        Assert.Contains(existingSource, actualSources);
        Assert.Contains(missingSource, actualSources);
    }

    [Fact]
    public async Task AddMissingAsync_AllSourcesAlreadyExist_ReturnsSuccessWithoutChanges()
    {
        var existingSource1 = NewSource();
        var existingSource2 = NewSource();

        Assert.True((await _service.AddMissingAsync(new SourceAddMissing.Command([existingSource1, existingSource2]))).IsSucc);

        var result = await _service.AddMissingAsync(new SourceAddMissing.Command([existingSource1, existingSource2]));
        Assert.True(result.IsSucc);

        var actual = await _getByNamesService.GetByNamesAsync(new GetByNamesSource.Command([existingSource1.Name, existingSource2.Name]));
        Assert.True(actual.IsSucc);
        var actualSources = actual.ToOption().ValueUnsafe();

        Assert.Equal(2, actualSources.Count);
        Assert.Contains(existingSource1, actualSources);
        Assert.Contains(existingSource2, actualSources);
    }
}
