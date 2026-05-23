using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Account;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.Update;

public class Service(IBulkStore bulkStore)
{
    public Task<Fin<Unit>> UpdateAsync(Command command) => bulkStore.UpdateAsync(command.Accounts);
}
