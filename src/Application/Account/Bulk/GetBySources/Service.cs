using LanguageExt;
using LanguageExt.Common;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySources;

public class Service(GetByNamesSource.Service getByNamesSource, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Account.Account>>> GetBySourcesAsync(Command command)
    {
        var sourceNames = command.Sources.Select(i => i.Name).ToList();
        return getByNamesSource.GetByNamesAsync(new GetByNamesSource.Command(sourceNames))
            
            .BindAsync(bulkStore.GetBySourcesAsync)
            .BindAsync(s => s.Count == 0 ? Error.New("accounts. not found") :s.ToFin());
    }
}
