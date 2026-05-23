using LanguageExt;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Domain.ValueObjects;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Instrument.Post;

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
