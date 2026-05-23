using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source.Post;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Source.Source>> PostAsync(Command command)
    {
        return persistenceStore.GetByNameAsync(command.Name)
            .MatchAsync(i => i.ToFinAsync(), PostAsync(command.Name));
    }

    private Func<Task<Fin<Domain.Source.Source>>> PostAsync(Domain.ValueObjects.SourceName name) => () =>
        persistenceStore.CreateAsync(new Domain.Source.Source(SourceId.New(), name));
}
