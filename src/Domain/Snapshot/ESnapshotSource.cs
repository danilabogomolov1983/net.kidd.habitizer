using System.Runtime.Serialization;

namespace Wst.Tools.PosiBridge.Domain.Snapshot;

public enum ESnapshotSource
{
    [EnumMember(Value = "TRADIX")]
    Tradix,

    [EnumMember(Value = "TICK_TS")]
    TickTs
}
