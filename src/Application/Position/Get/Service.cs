using LanguageExt;
using AccountGet = Wst.Tools.PosiBridge.Application.Account.Get;
using InstrumentGet = Wst.Tools.PosiBridge.Application.Instrument.Get;
using DomainPosition = Wst.Tools.PosiBridge.Domain.Position.Position;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using IPersistenceStore = Wst.Tools.PosiBridge.Domain.Position.IPersistenceStore;

namespace Wst.Tools.PosiBridge.Application.Position.Get;

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
