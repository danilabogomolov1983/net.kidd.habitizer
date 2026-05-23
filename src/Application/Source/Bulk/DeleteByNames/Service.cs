using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source.Bulk.DeleteByNames;

public class Service(GetByNames.Service getByNamesService, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command) =>
        getByNamesService.GetByNamesAsync(new GetByNames.Command(command.SourceNames))
            .BindAsync(i =>
            {
                var names = i.Select(k => k.Name).ToList();
                return bulkStore.DeleteByNamesAsync(names);
            });
}
