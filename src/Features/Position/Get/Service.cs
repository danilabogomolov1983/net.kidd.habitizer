using LanguageExt;
using AccountGet = Net.Kidd.Habitizer.Features.Account.Get;
using InstrumentGet = Net.Kidd.Habitizer.Features.Instrument.Get;
using DomainPosition = Net.Kidd.Habitizer.Domain.Position.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using IPersistenceStore = Net.Kidd.Habitizer.Domain.Position.IPersistenceStore;

namespace Net.Kidd.Habitizer.Features.Position.Get;

public class Service(IPersistenceStore persistenceStore, AccountGet.Service accountGetService, InstrumentGet.Service instrumentGetService)
{
    public Task<Fin<DomainPosition>> GetAsync(Command command)
    {
        var instrumentGetCommand = new InstrumentGet.Command(command.Isin);
        var accountGetCommand = new AccountGet.Command(command.SourceName, command.AccountName);
        return instrumentGetService.GetAsync(instrumentGetCommand)
                .BindAsync(instrument => accountGetService.GetAsync(accountGetCommand)
                    .BindAsync(account => persistenceStore.GetByIdAsync(account.Id, instrument.Id).ToFinAsync("Position not found")));
    }
}
