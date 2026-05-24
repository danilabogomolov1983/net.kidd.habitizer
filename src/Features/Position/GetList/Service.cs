using LanguageExt;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Position;

namespace Net.Kidd.Habitizer.Features.Position.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Position.Position>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


