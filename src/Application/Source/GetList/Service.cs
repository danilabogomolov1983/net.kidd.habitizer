using LanguageExt;
using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Source;

namespace Wst.Tools.PosiBridge.Application.Source.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Source.Source>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


