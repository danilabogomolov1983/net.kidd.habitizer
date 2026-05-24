using LanguageExt;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Position.Bulk.AddMissing;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<Unit>> AddMissingAsync(Command command)
    {
        var accountIds = command.Positions
            .Select(position => position.Account.Id)
            .Distinct()
            .ToList();

        var existingPositions = await bulkStore.GetByAccountIdsAsync(accountIds);
        var missingPositions = command.Positions
            .ExceptBy(existingPositions.Select(position => (position.Account, position.Instrument)),
                position => (position.Account, position.Instrument))
            .ToList();

        if (missingPositions.Count == 0)
        {
            return await Unit.SuccessAsync();
        }

        return await bulkStore.InsertAsync(missingPositions);
    }
}
