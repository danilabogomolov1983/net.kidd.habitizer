using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Instrument;
using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Instrument.Post;

public class Service(IPersistenceStore persistenceStore)
{
    public Task<Fin<Domain.Instrument.Instrument>> PostAsync(Command command)
    {
        return persistenceStore.GetByIsinAsync(command.Isin)
            .MatchAsync(i => i.ToFinAsync(), PostAsync(command.Isin));
    }

    private Func<Task<Fin<Domain.Instrument.Instrument>>> PostAsync(Isin isin) => () =>
        persistenceStore.CreateAsync(new Domain.Instrument.Instrument(InstrumentId.New(), isin));
}
