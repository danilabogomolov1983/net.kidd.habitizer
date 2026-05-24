using LanguageExt;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<IReadOnlyList<Domain.Account.Account>>> GetByIdsAsync(Command command)
    {
        var accountIds = command.AccountIds;
        var existingAccounts = await bulkStore.GetByIdsAsync(accountIds);

        var notFound = accountIds
            .Except(existingAccounts.Select(i => i.Id))
            .ToList();
        return notFound.Count > 0 ? Fin<IReadOnlyList<Domain.Account.Account>>.Fail("Some queried accounts were not found.") : existingAccounts.ToFin();
    }
}
