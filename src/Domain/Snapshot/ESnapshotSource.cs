using System.Runtime.Serialization;

namespace Net.Kidd.Habitizer.Domain.Snapshot;

public enum ESnapshotSource
{
    [EnumMember(Value = "TRADIX")]
    Tradix,

    [EnumMember(Value = "TICK_TS")]
    TickTs
}
