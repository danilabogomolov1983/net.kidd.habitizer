using LanguageExt;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Account;

public interface IPersistenceStore
{
    Task<Fin<Account>> CreateAsync(Account account);
    Task<Fin<Account>> UpdateAsync(Account account);
    Task<Option<Account>> GetByIdAsync(AccountId accountId);
    Task<Option<Account>> GetByNameAsync(AccountName name, Source.SourceId sourceId);
    Task<IReadOnlyList<Account>> GetBySourceAsync(Source.SourceId sourceId, int pageNumber, int pageSize);
    Task<IReadOnlyList<Account>> GetListAsync(int pageNumber, int pageSize);
}
