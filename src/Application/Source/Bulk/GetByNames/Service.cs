using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<IReadOnlyList<Domain.Source.Source>>> GetByNamesAsync(Command command)
    {
        var sourceNames = command.SourceNames;
        var existingSources = await bulkStore.GetByNamesAsync(command.SourceNames);

        var notFound = sourceNames
            .Except(existingSources.Select(i => i.Name))
            .ToList();
        return notFound.Count > 0 ? Fin<IReadOnlyList<Domain.Source.Source>>.Fail("Some queried sources were not found.") : existingSources.ToFin();
    }
}
