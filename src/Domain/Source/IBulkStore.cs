using LanguageExt;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Source;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Source> sources);
    Task<IReadOnlyList<Source>> GetByNamesAsync(IReadOnlyList<SourceName> sourceNames);
    Task<Fin<Unit>> DeleteByNamesAsync(IReadOnlyList<SourceName> sourceNames);
}
