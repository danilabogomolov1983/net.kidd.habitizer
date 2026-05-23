using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.TestCompanion;
using SourceAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Source.Bulk.AddMissing;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly SourceAddMissing.Service _service = fixture.GetRequiredService<SourceAddMissing.Service>();
    private readonly GetByNamesSource.Service _getByNamesService = fixture.GetRequiredService<GetByNamesSource.Service>();

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
}
