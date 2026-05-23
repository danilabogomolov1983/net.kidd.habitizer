using LanguageExt;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using GetByNamesSource = Net.Kidd.Habitizer.Application.Source.Bulk.GetByNames;

namespace Net.Kidd.Habitizer.Application.Position.Bulk.DeleteBySource;

public class Service(GetByNamesSource.Service getByNamesSource, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteBySourcesAsync(Command command)
    {
        var sourceNames = command.Sources.Select(i => i.Name).ToList();
        return getByNamesSource.GetByNamesAsync(new GetByNamesSource.Command(sourceNames))
            .BindAsync(bulkStore.DeleteBySourceAsync);
    }
}
