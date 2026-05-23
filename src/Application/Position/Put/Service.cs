using LanguageExt;
using AccountPost = Wst.Tools.PosiBridge.Application.Account.Post;
using DomainPosition = Wst.Tools.PosiBridge.Domain.Position.Position;
using InstrumentPost = Wst.Tools.PosiBridge.Application.Instrument.Post;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;
using IPersistenceStore = Wst.Tools.PosiBridge.Domain.Position.IPersistenceStore;

namespace Wst.Tools.PosiBridge.Application.Position.Put;

public class Service(
    IPersistenceStore persistenceStore,
    AccountPost.Service accountPostService,
    InstrumentPost.Service instrumentPostService)
{
    public Task<Fin<DomainPosition>> PutAsync(Command command)
    {
        var accountPostCommand = new AccountPost.Command(command.SourceName, command.AccountName);
        var instrumentPostCommand = new InstrumentPost.Command(command.Isin);

        return instrumentPostService.PostAsync(instrumentPostCommand)
            .BindAsync(instrument =>
                accountPostService.PostAsync(accountPostCommand)
                    .BindAsync(account =>
                    {
                        var position = new DomainPosition(
                            Account: account,
                            Instrument: instrument,
                            NetSize: command.NetSize,
                            NetValue: command.NetValue,
                            UnrealisedAverageCost: command.UnrealisedAverageCost,
                            UnrealisedProfit: command.UnrealisedProfit,
                            UnrealisedProfitPercent: command.UnrealisedProfitPercent,
                            ReferencePrice: command.ReferencePrice);

                        return persistenceStore.GetByIdAsync(account.Id, instrument.Id)
                            .MatchAsync(
                                _ => persistenceStore.UpdateAsync(position),
                                () => persistenceStore.CreateAsync(position));
                    }));
    }
}
