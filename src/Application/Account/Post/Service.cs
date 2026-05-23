using LanguageExt;
using DomainAccount = Wst.Tools.PosiBridge.Domain.Account.Account;
using SourcePost = Wst.Tools.PosiBridge.Application.Source.Post;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Account.Post;

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
