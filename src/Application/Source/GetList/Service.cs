using LanguageExt;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Source;

namespace Net.Kidd.Habitizer.Application.Source.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Source.Source>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


