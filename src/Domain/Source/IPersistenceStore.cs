using LanguageExt;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Source;

public interface IPersistenceStore
{
    Task<Fin<Source>> CreateAsync(Source source);
    Task<Option<Source>> GetByIdAsync(SourceId sourceId);
    Task<Option<Source>> GetByNameAsync(SourceName name);
    Task<IReadOnlyList<Source>> GetListAsync(int pageNumber, int pageSize);
}
