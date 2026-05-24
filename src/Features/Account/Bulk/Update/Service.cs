using LanguageExt;
using Net.Kidd.Habitizer.Domain.Account;

namespace Net.Kidd.Habitizer.Features.Account.Bulk.Update;

public class Service(IBulkStore bulkStore)
{
    public Task<Fin<Unit>> UpdateAsync(Command command) => bulkStore.UpdateAsync(command.Accounts);
}
