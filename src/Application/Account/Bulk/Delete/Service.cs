using LanguageExt;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Account.Bulk.Delete;

public class Service(GetByIds.Service getByIdsService, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command)
    {
        var ids = command.Accounts.Select(i => i.Id).ToList();
        
        return getByIdsService.GetByIdsAsync(new GetByIds.Command(ids))
            .BindAsync(bulkStore.DeleteAsync);
    }
}
