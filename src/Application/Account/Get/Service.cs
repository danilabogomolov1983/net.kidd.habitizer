using LanguageExt;
using DomainAccount = Net.Kidd.Habitizer.Domain.Account.Account;
using SourceGet = Net.Kidd.Habitizer.Application.Source.Get;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Account.Get;

public class Service(IPersistenceStore persistenceStore, SourceGet.Service sourceGetService)
{
    public Task<Fin<DomainAccount>> GetAsync(Command command) =>
        sourceGetService
            .GetAsync(new SourceGet.Command(command.SourceName))
            .BindAsync(s => persistenceStore.GetByNameAsync(command.Name, s.Id)
                            .ToFinAsync("Account not found"));
}
