using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.DeleteBySource;

public class Service(GetByNamesSource.Service getByNamesSource, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command)
    {
        var sourceNames = command.Sources.Select(i => i.Name).ToList();
        return getByNamesSource.GetByNamesAsync(new GetByNamesSource.Command(sourceNames))
            .BindAsync(bulkStore.DeleteBySourceAsync);
    }
}
