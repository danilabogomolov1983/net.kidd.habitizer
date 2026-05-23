using LanguageExt;
using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Position;

namespace Wst.Tools.PosiBridge.Application.Position.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Position.Position>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize).Map(i => i.ToImmutableList());
}


