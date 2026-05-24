using LanguageExt;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using AccountBulkAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;
using AccountBulkGetBySourceAndNames = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySourceAndNames;
using AccountBulkUpdate = Net.Kidd.Habitizer.Features.Account.Bulk.Update;
using InstrumentBulkAddMissing = Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;
using PositionBulkAddMissing = Net.Kidd.Habitizer.Features.Position.Bulk.AddMissing;
using PositionBulkDeleteBySource = Net.Kidd.Habitizer.Features.Position.Bulk.DeleteBySource;
using SourceBulkAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using DomainAccount = Net.Kidd.Habitizer.Domain.Account.Account;
using DomainSource = Net.Kidd.Habitizer.Domain.Source.Source;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Snapshot.Merge;

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
