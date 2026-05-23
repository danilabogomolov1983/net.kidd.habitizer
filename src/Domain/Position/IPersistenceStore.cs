using LanguageExt;

namespace Wst.Tools.PosiBridge.Domain.Position;

public interface IPersistenceStore
{
    Task<Fin<Position>> CreateAsync(Position position);
    Task<Fin<Position>> UpdateAsync(Position position);
    Task<Option<Position>> GetByIdAsync(Account.AccountId accountId, Instrument.InstrumentId instrumentId);
    Task<IReadOnlyList<Position>> GetListAsync(int pageNumber, int pageSize);
}
