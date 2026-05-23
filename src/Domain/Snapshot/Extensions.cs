using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Serialization;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Snapshot;

public static class SnapshotExtensions
{
    public static SourceName ToSourceName(this ESnapshotSource snapshotSource)
    {
        var configuredName = typeof(ESnapshotSource)
            .GetField(snapshotSource.ToString())
            ?.GetCustomAttribute<EnumMemberAttribute>()
            ?.Value;

        return new SourceName(configuredName ?? snapshotSource.ToString());
    }
    extension(Snapshot snapshot)
    {
        
        public Snapshot WithSource(ESnapshotSource source) => snapshot with { Source = source };

        public Snapshot WithPositions(IImmutableList<Position.Position> positions) =>
            snapshot with { Positions = positions };
    }
}
