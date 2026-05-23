using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Position;
using Wst.Tools.PosiBridge.Domain.Snapshot;

namespace Wst.Tools.PosiBridge.TestCompanion.Snapshot;

public static class Support
{
    public static class Domain
    {
        public static PosiBridge.Domain.Snapshot.Snapshot NewSnapshot(ESnapshotSource snapshotSource, PosiBridge.Domain.Account.Account account, int positionsCount)
        {
            var positions = Enumerable.Range(0, positionsCount)
                .Select(_ => Position.Support.Domain.NewPosition().WithAccount(account))
                .ToImmutableList();

            return new PosiBridge.Domain.Snapshot.Snapshot(snapshotSource, positions);
        }

    }
}
