using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source.Get;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Source.Source>> GetAsync(Command command) =>
        persistenceStore.GetByNameAsync(command.Name).ToFinAsync("Source not found");
}
