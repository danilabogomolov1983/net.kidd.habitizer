using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.Delete;

public class Service(GetByIds.Service getByIdsService, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command)
    {
        var ids = command.Accounts.Select(i => i.Id).ToList();
        
        return getByIdsService.GetByIdsAsync(new GetByIds.Command(ids))
            .BindAsync(bulkStore.DeleteAsync);
    }
}
