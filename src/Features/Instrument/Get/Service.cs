using LanguageExt;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Instrument.Get;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Instrument.Instrument>> GetAsync(Command command) =>
        persistenceStore.GetByIsinAsync(command.Isin).ToFinAsync("Instrument not found");
}
