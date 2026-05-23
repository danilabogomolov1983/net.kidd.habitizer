using System.Collections.Immutable;

namespace Wst.Tools.PosiBridge.TickTs.Snapshot;

public sealed record Response(IImmutableList<Position.Response> Positions);
