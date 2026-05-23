using Microsoft.Extensions.Options;
using Net.Kidd.Habitizer.Application.Snapshot.Configuration;
using Net.Kidd.Habitizer.Application.Snapshot.Sync;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.TestCompanion;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Snapshot.Sync;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    [Theory]
    [InlineData(ESnapshotSource.TickTs)]
    [InlineData(ESnapshotSource.Tradix)]
    public async Task SyncAsync_MergesSnapshotsForConfiguredSourceAccounts(ESnapshotSource source)
    {
        var settings = fixture.GetRequiredService<IOptions<SyncSettings>>();
        var accounts = settings.Value.GetAccounts(source);
        var service = fixture.GetRequiredService<Service>();
        var maybe = await service.SyncAsync(source, accounts);

        maybe.IfFail(err =>
        {
            output.WriteLine(err.Message);
        });
        Assert.True(maybe.IsSucc);
    }
}
