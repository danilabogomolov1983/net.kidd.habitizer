using LanguageExt;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Instrument;

public interface IPersistenceStore
{
    Task<Fin<Instrument>> CreateAsync(Instrument instrument);
    Task<Option<Instrument>> GetByIdAsync(InstrumentId instrumentId);
    Task<Option<Instrument>> GetByIsinAsync(Isin isin);
    Task<IReadOnlyList<Instrument>> GetListAsync(int pageNumber, int pageSize);
}

