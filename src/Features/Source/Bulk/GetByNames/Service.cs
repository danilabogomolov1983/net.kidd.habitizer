using LanguageExt;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;

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
