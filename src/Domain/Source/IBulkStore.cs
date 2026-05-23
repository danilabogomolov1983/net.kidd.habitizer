using LanguageExt;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Source;

public interface IBulkStore
{
    Task<Fin<Unit>> InsertAsync(IReadOnlyList<Source> sources);
    Task<IReadOnlyList<Source>> GetByNamesAsync(IReadOnlyList<SourceName> sourceNames);
    Task<Fin<Unit>> DeleteByNamesAsync(IReadOnlyList<SourceName> sourceNames);
}
