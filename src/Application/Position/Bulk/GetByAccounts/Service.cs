using LanguageExt;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using AccountGetByIds = Net.Kidd.Habitizer.Application.Account.Bulk.GetByIds;

namespace Net.Kidd.Habitizer.Application.Position.Bulk.GetByAccounts;

public class Service(AccountGetByIds.Service getByIdsAccount, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Position.Position>>> GetByAccountAsync(Command command)
    {
        var accountIds = command.Accounts.Select(i => i.Id).ToList();
        return getByIdsAccount.GetByIdsAsync(new AccountGetByIds.Command(accountIds))
            .MapAsync(i => i.Map(k => k.Id).ToList())
            .BindAsync(bulkStore.GetByAccountIdsAsync);
    }
}
