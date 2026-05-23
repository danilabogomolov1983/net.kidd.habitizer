using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Wst.Tools.PosiBridge.Application.Snapshot.Configuration;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Shared.Kernel.Errors;

namespace Wst.Tools.PosiBridge.SyncDaemon.Workers;

[DisallowConcurrentExecution]
public sealed class TradixSyncWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<SyncSettings> syncSettings,
    ILogger<TradixSyncWorker> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        if (!syncSettings.Value.Accounts.ContainsKey(ESnapshotSource.Tradix))
        {
            logger.LogDebug(
                "[Step:Skipped] No accounts configured for snapshot source {SnapshotSource}",
                ESnapshotSource.Tradix);
            return;
        }

        var accounts = syncSettings.Value.GetAccounts(ESnapshotSource.Tradix);

        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["SnapshotSource"] = ESnapshotSource.Tradix,
            ["Accounts"] = accounts.ToFormattedString((item, _) => item),
            ["AccountsCount"] = accounts.Length,
        });

        using var scope = scopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<Application.Snapshot.Sync.Service>();
        var result = await syncService.SyncAsync(ESnapshotSource.Tradix, accounts);

        result.Match(_ =>
        {
            logger.LogInformation(
                "[Step:Completed] Synchronized snapshot source {SnapshotSource}",
                ESnapshotSource.Tradix);
        }, err =>
        {
            logger.LogError(
                "[Step:Failed] Synchronization failed for source {SnapshotSource}. Message: {Message}",
                ESnapshotSource.Tradix,
                err.Message);
        });
    }
}
