using System.Collections.Immutable;

namespace Net.Kidd.Habitizer.TickTs.Snapshot;

public sealed record Response(IImmutableList<Position.Response> Positions);
