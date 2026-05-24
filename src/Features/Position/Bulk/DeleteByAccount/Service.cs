using LanguageExt;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using AccountGetByIds = Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;

namespace Net.Kidd.Habitizer.Features.Position.Bulk.DeleteByAccount;

public class Service(AccountGetByIds.Service getByIdsAccount, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command)
    {
        var accountIds = command.Accounts.Select(i => i.Id).ToList();
        return getByIdsAccount.GetByIdsAsync(new AccountGetByIds.Command(accountIds))
            .BindAsync(bulkStore.DeleteByAccountAsync);
    }
}
