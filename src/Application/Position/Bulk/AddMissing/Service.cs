using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Position;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;

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
