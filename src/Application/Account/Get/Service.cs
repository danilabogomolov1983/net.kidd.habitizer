using LanguageExt;
using DomainAccount = Wst.Tools.PosiBridge.Domain.Account.Account;
using SourceGet = Wst.Tools.PosiBridge.Application.Source.Get;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Account.Get;

public class Service(IPersistenceStore persistenceStore, SourceGet.Service sourceGetService)
{
    public Task<Fin<DomainAccount>> GetAsync(Command command) =>
        sourceGetService
            .GetAsync(new SourceGet.Command(command.SourceName))
            .BindAsync(s => persistenceStore.GetByNameAsync(command.Name, s.Id)
                            .ToFinAsync("Account not found"));
}
