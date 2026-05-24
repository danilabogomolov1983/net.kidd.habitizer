using LanguageExt;
using DomainAccount = Net.Kidd.Habitizer.Domain.Account.Account;
using SourceGet = Net.Kidd.Habitizer.Features.Source.Get;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using IPersistenceStore = Net.Kidd.Habitizer.Domain.Account.IPersistenceStore;

namespace Net.Kidd.Habitizer.Features.Account.Put;

public class Service(IPersistenceStore persistenceStore, SourceGet.Service sourceGetService)
{
    public Task<Fin<DomainAccount>> PutAsync(Command command)
    {
        return sourceGetService
            .GetAsync(new SourceGet.Command(command.SourceName))
            .BindAsync(s => persistenceStore.GetByNameAsync(command.AccountName, s.Id)
                    .MatchAsync(existing =>
                        {
                            existing.WithLastUpdatedAt(command.LastUpdatedAt);
                            return persistenceStore.UpdateAsync(existing);
                        },
                        () =>
                        {
                            var newAccount = new DomainAccount(AccountId.New(),
                                s,
                                command.AccountName);
                            return persistenceStore.CreateAsync(newAccount);
                        }));
    }
}
