using LanguageExt;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<Unit>> AddMissingAsync(Command command)
    {
        var accountsGroupedBySource = command.Accounts
            .GroupBy(i=>i.Source)
            .Select(g => (g.Key.Name,
                                                        g.GroupBy(account => account.Name)
                                                        .Select(accountName=> accountName.Key)))
            .ToList();

        var existingAccounts = new List<Domain.Account.Account>();
        foreach (var groupAndValues in accountsGroupedBySource)
        {
            var existing = await bulkStore.GetBySourceAndNamesAsync(groupAndValues.Name, groupAndValues.Item2.ToImmutableList());
            existingAccounts.AddRange(existing);            
        }
        
        var missingAccounts = command.Accounts
            .ExceptBy(existingAccounts.Select(account => (account.Source.Name, account.Name)), account => (account.Source.Name, account.Name))
            .ToList();

        if (missingAccounts.Count == 0)
        {
            return await Unit.SuccessAsync();
        }
        return await bulkStore.InsertAsync(missingAccounts);
    }
}
