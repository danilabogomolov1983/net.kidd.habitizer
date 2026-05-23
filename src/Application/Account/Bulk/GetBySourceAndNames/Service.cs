using LanguageExt;
using LanguageExt.Common;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using GetSource = Wst.Tools.PosiBridge.Application.Source.Get;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySourceAndNames;

public class Service(GetSource.Service getSource, IBulkStore bulkStore)
{
    public Task<Fin<IReadOnlyList<Domain.Account.Account>>> GetBySourceAndNamesAsync(Command command) =>
        getSource.GetAsync(new GetSource.Command(command.SourceName))
            .BindAsync(source => bulkStore.GetBySourceAndNamesAsync(source.Name, command.AccountNames))
            .BindAsync(s => s.Count == 0 ? Error.New("accounts. not found") :s.ToFin());
}
