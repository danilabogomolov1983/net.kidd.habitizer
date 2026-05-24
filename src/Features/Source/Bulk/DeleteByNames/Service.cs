using LanguageExt;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Source.Bulk.DeleteByNames;

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
