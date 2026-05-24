using LanguageExt;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Source.Get;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Source.Source>> GetAsync(Command command) =>
        persistenceStore.GetByNameAsync(command.Name).ToFinAsync("Source not found");
}
