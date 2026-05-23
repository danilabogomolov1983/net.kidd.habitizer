using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Position;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using GetByNamesSource = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

namespace Wst.Tools.PosiBridge.Application.Position.Bulk.GetBySource;

public class Service(GetByNamesSource.Service getByNamesSource, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Position.Position>>> GetBySourceAsync(Command command)
    {
        var sourceNames = command.Sources.Select(i => i.Name).ToList();
        return getByNamesSource.GetByNamesAsync(new GetByNamesSource.Command(sourceNames))
            .BindAsync(bulkStore.GetBySourceAsync);
    }
}
