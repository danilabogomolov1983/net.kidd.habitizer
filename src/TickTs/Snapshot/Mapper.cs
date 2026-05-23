using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel;

namespace Wst.Tools.PosiBridge.TickTs.Snapshot;

public struct Mapper : IOnewayMap<Domain.Snapshot.Snapshot, (string, Response)>
{
    public Domain.Snapshot.Snapshot Map((string, Response) right)
    {
        var (accountName, response) = right;
        var sourceName = ESnapshotSource.TickTs.ToSourceName();
        var source = new Domain.Source.Source(Domain.Source.SourceId.Empty(), sourceName);
        var account = new Domain.Account.Account(Domain.Account.AccountId.Empty(), source, new AccountName(accountName));
        var positions = response.Positions
            .Map(p => TickTsMappers.SnapshotPositionMapper.Map((account, p)))
            .ToImmutableList();


        return new Domain.Snapshot.Snapshot(ESnapshotSource.TickTs, positions);
    }
}
