using System.Collections.Immutable;

namespace Net.Kidd.Habitizer.Domain.Snapshot;

public sealed record Snapshot(ESnapshotSource Source, IImmutableList<Position.Position> Positions);
