using LanguageExt;
using LanguageExt.Common;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using GetSource = Net.Kidd.Habitizer.Features.Source.Get;

namespace Net.Kidd.Habitizer.Features.Account.Bulk.GetBySourceAndNames;

public class Service(GetSource.Service getSource, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Account.Account>>> GetBySourceAndNamesAsync(Command command) =>
        getSource.GetAsync(new GetSource.Command(command.SourceName))
            .BindAsync(source => bulkStore.GetBySourceAndNamesAsync(source.Name, command.AccountNames))
            .BindAsync(s => s.Count == 0 ? Error.New("accounts. not found") :s.ToFin());
}
