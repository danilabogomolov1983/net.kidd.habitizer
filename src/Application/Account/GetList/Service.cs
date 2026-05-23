using LanguageExt;
using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Account;

namespace Wst.Tools.PosiBridge.Application.Account.GetList;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<ImmutableList<Domain.Account.Account>> GetListAsync(Command command) => persistenceStore
        .GetListAsync(command.PageNumber, command.PageSize)
        .Map(i => i.ToImmutableList());
}


