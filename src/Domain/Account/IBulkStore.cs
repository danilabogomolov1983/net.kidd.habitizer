using LanguageExt;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Account;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Account> accounts);
    Task<Fin<Unit>> UpdateAsync(IReadOnlyList<Domain.Account.Account> accounts);
    Task<IReadOnlyList<Account>> GetByIdsAsync(IReadOnlyList<AccountId> accountIds);
    Task<IReadOnlyList<Account>> GetBySourcesAsync(IReadOnlyList<Source.Source> sources);
    Task<IReadOnlyList<Account>> GetBySourceAndNamesAsync(SourceName sourceName, IReadOnlyList<AccountName> accountNames);
    Task<Fin<Unit>> DeleteAsync(IReadOnlyList<Account> accountNames);
    Task<Fin<Unit>> DeleteByIdsAsync(IReadOnlyList<AccountId> accountIds);
    Task<Fin<Unit>> DeleteBySourceAsync(IReadOnlyList<Source.Source> sources);
}
