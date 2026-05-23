using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Instrument;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Instrument.Get;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Instrument.Instrument>> GetAsync(Command command) =>
        persistenceStore.GetByIsinAsync(command.Isin).ToFinAsync("Instrument not found");
}
