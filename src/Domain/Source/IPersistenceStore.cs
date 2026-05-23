using LanguageExt;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Source;

public interface IPersistenceStore
{
    Task<Fin<Source>> CreateAsync(Source source);
    Task<Option<Source>> GetByIdAsync(SourceId sourceId);
    Task<Option<Source>> GetByNameAsync(SourceName name);
    Task<IReadOnlyList<Source>> GetListAsync(int pageNumber, int pageSize);
}
