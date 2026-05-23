using LanguageExt;
using DomainAccount = Wst.Tools.PosiBridge.Domain.Account.Account;
using SourceGet = Wst.Tools.PosiBridge.Application.Source.Get;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using IPersistenceStore = Wst.Tools.PosiBridge.Domain.Account.IPersistenceStore;

namespace Wst.Tools.PosiBridge.Application.Account.Put;

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
