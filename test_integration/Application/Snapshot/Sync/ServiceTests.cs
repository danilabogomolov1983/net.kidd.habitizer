using Microsoft.Extensions.Options;
using Wst.Tools.PosiBridge.Application.Snapshot.Configuration;
using Wst.Tools.PosiBridge.Application.Snapshot.Sync;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.TestCompanion;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Snapshot.Sync;

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
