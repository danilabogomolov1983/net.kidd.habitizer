using LanguageExt;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using AccountBulkAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using AccountBulkGetBySourceAndNames = Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySourceAndNames;
using AccountBulkUpdate = Wst.Tools.PosiBridge.Application.Account.Bulk.Update;
using InstrumentBulkAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;
using PositionBulkAddMissing = Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;
using PositionBulkDeleteBySource = Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteBySource;
using SourceBulkAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using DomainAccount = Wst.Tools.PosiBridge.Domain.Account.Account;
using DomainSource = Wst.Tools.PosiBridge.Domain.Source.Source;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Snapshot.Merge;

public class Service(
    SourceBulkAddMissing.Service sourceAddMissingService,
    InstrumentBulkAddMissing.Service instrumentAddMissingService,
    AccountBulkAddMissing.Service accountAddMissingService,
    AccountBulkGetBySourceAndNames.Service accountGetBySourceAndNamesService,
    AccountBulkUpdate.Service accountUpdateService,
    PositionBulkAddMissing.Service positionAddMissingService,
    PositionBulkDeleteBySource.Service positionDeleteBySourceService,
    TimeProvider timeProvider,
    ILogger<Service> logger)
{
    public Task<Fin<Unit>> MergeAsync(Command command)
    {
        var (snapshotSource, positions) = command.Snapshot;
        var accounts = positions.Select(i => i.Account)
            .Distinct()
            .ToImmutableList();
        var instruments = positions.Select(i => i.Instrument)
            .Distinct()
            .ToImmutableList();

        var source = new DomainSource(SourceId.New(), snapshotSource.ToSourceName());

        return sourceAddMissingService.AddMissingAsync(new SourceBulkAddMissing.Command([source]))
            .BindAsync(_ => accountAddMissingService.AddMissingAsync(new AccountBulkAddMissing.Command(accounts)))
            .BindAsync(_ =>
                instrumentAddMissingService.AddMissingAsync(new InstrumentBulkAddMissing.Command(instruments)))
            .BindAsync(_ =>
                positionDeleteBySourceService.DeleteBySourcesAsync(new PositionBulkDeleteBySource.Command([source])))
            .BindAsync(_ => positionAddMissingService.AddMissingAsync(new PositionBulkAddMissing.Command(positions)))
            .MatchAsync(_ => UpdateAccountLastUpdatedAt(source, accounts),
                err =>
                {
                    logger.LogError("Failed to save snapshot position for source {TargetSource}", source);
                    return Unit.FailureAsync(err);
                });
    }

    private Task<Fin<Unit>> UpdateAccountLastUpdatedAt(DomainSource source, ImmutableList<DomainAccount> accounts)
    {
        var accountsNames = accounts.Select(i => i.Name).ToImmutableList();
        return accountGetBySourceAndNamesService.GetBySourceAndNamesAsync(new AccountBulkGetBySourceAndNames.Command(source.Name, accountsNames))
            .BindAsync(i =>
            {
                var updatedAccounts = i
                    .Map(i => i.WithLastUpdatedAt(timeProvider.GetLocalNow()))
                    .ToImmutableList();
                return accountUpdateService.UpdateAsync(
                    new AccountBulkUpdate.Command(source.Name, updatedAccounts));
            });
    }
}
