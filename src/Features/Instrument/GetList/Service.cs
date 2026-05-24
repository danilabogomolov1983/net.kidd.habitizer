using LanguageExt;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Instrument;

namespace Net.Kidd.Habitizer.Features.Instrument.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Instrument.Instrument>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


