using LanguageExt;
using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Instrument;

namespace Wst.Tools.PosiBridge.Application.Instrument.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Instrument.Instrument>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


