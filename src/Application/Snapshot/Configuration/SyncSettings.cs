using Wst.Tools.PosiBridge.Domain.Snapshot;

namespace Wst.Tools.PosiBridge.Application.Snapshot.Configuration;

public class SyncSettings
{
    public const string ConfigurationSection = "SyncSettings";

    public SourceAccounts Accounts { get; init; } = [];

    public string[] GetAccounts(ESnapshotSource snapshotSource) => Accounts[snapshotSource].ToArray();
}

public class SourceAccounts : Dictionary<ESnapshotSource, string[]>;

