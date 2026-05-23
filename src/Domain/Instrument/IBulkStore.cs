using LanguageExt;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Instrument;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Instrument> instruments);
    Task<IReadOnlyList<Instrument>> GetByIsinsAsync(IReadOnlyList<Isin> isins);
    Task<Fin<Unit>> DeleteByIsinsAsync(IReadOnlyList<Isin> isins);
}
