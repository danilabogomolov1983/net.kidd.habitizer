using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Domain.Snapshot;

namespace Net.Kidd.Habitizer.TestCompanion.Snapshot;

public static class Support
{
    public static class Domain
    {
        public static Habitizer.Domain.Snapshot.Snapshot NewSnapshot(ESnapshotSource snapshotSource, Habitizer.Domain.Account.Account account, int positionsCount)
        {
            var positions = Enumerable.Range(0, positionsCount)
                .Select(_ => Position.Support.Domain.NewPosition().WithAccount(account))
                .ToImmutableList();

            return new Habitizer.Domain.Snapshot.Snapshot(snapshotSource, positions);
        }

    }
}
