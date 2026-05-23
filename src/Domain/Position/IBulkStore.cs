using LanguageExt;

namespace Wst.Tools.PosiBridge.Domain.Position;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Position> positions);
    Task<IReadOnlyList<Position>> GetByAccountIdsAsync(IReadOnlyList<Account.AccountId> accountIds);
    Task<IReadOnlyList<Position>> GetBySourceAsync(IReadOnlyList<Source.Source> sources);
    Task<Fin<Unit>> DeleteByAccountAsync(IReadOnlyList<Account.Account> accounts);
    Task<Fin<Unit>> DeleteBySourceAsync(IReadOnlyList<Domain.Source.Source> sources);
    
}
