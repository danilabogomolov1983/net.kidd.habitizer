using System.Collections.Immutable;

namespace Wst.Tools.PosiBridge.Domain.Snapshot;

public sealed record Snapshot(ESnapshotSource Source, IImmutableList<Position.Position> Positions);
