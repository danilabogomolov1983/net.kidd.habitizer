using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Position;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using AccountGetByIds = Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;

namespace Wst.Tools.PosiBridge.Application.Position.Bulk.GetByAccounts;

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
