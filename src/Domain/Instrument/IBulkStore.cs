using LanguageExt;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Instrument;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Instrument> instruments);
    Task<IReadOnlyList<Instrument>> GetByIsinsAsync(IReadOnlyList<Isin> isins);
    Task<Fin<Unit>> DeleteByIsinsAsync(IReadOnlyList<Isin> isins);
}
