using LanguageExt;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using GetByNamesSource = Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;

namespace Net.Kidd.Habitizer.Features.Position.Bulk.GetBySource;

public class Service(GetByNamesSource.Service getByNamesSource, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Position.Position>>> GetBySourceAsync(Command command)
    {
        var sourceNames = command.Sources.Select(i => i.Name).ToList();
        return getByNamesSource.GetByNamesAsync(new GetByNamesSource.Command(sourceNames))
            .BindAsync(bulkStore.GetBySourceAsync);
    }
}
