using LanguageExt;
using LanguageExt.Common;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using GetByNamesSource = Net.Kidd.Habitizer.Application.Source.Bulk.GetByNames;

namespace Net.Kidd.Habitizer.Application.Account.Bulk.GetBySources;

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
