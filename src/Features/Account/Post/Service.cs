using LanguageExt;
using DomainAccount = Net.Kidd.Habitizer.Domain.Account.Account;
using SourcePost = Net.Kidd.Habitizer.Features.Source.Post;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Account.Post;

public class Service(IPersistenceStore persistenceStore, SourcePost.Service sourcePostService)
{
    public Task<Fin<DomainAccount>> PostAsync(Command command)
    {
        return sourcePostService.PostAsync(new SourcePost.Command(command.SourceName.Value))
            .BindAsync(s =>
            {
                return persistenceStore
                    .GetByNameAsync(command.AccountName, s.Id)
                    .MatchAsync(a => a.ToFinAsync(),
                        PostAsync(new DomainAccount(AccountId.New(), s, command.AccountName, command.LastUpdatedAt)));
            });
    }

    private Func<Task<Fin<DomainAccount>>> PostAsync(DomainAccount account) =>
        () => persistenceStore.CreateAsync(account);
}
