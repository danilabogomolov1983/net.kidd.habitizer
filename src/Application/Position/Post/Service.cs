using LanguageExt;
using AccountPost = Net.Kidd.Habitizer.Application.Account.Post;
using DomainPosition = Net.Kidd.Habitizer.Domain.Position.Position;
using InstrumentPost = Net.Kidd.Habitizer.Application.Instrument.Post;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using IPersistenceStore = Net.Kidd.Habitizer.Domain.Position.IPersistenceStore;

namespace Net.Kidd.Habitizer.Application.Position.Post;

public class Service(
    IPersistenceStore persistenceStore,
    AccountPost.Service accountPostService,
    InstrumentPost.Service instrumentPostService)
{
    public Task<Fin<DomainPosition>> PostAsync(Command command)
    {
        var accountPostCommand = new AccountPost.Command(command.SourceName, command.AccountName);
        var instrumentPostCommand = new InstrumentPost.Command(command.Isin);
        return instrumentPostService.PostAsync(instrumentPostCommand)
            .BindAsync(instrument => accountPostService.PostAsync(accountPostCommand)
                                        .BindAsync(account => persistenceStore.GetByIdAsync(account.Id, instrument.Id)
                                        .MatchAsync(
                                            p => p.ToFinAsync(),
                                            PostAsync(new DomainPosition
                                                                (
                                                                    Account: account,
                                                                    Instrument: instrument,
                                                                    NetSize: command.NetSize,
                                                                    NetValue: command.NetValue,
                                                                    UnrealisedAverageCost: command.UnrealisedAverageCost,
                                                                    UnrealisedProfit: command.UnrealisedProfit,
                                                                    UnrealisedProfitPercent: command.UnrealisedProfitPercent,
                                                                    ReferencePrice: command.ReferencePrice
                                            ))))
            );
    }
    
    private Func<Task<Fin<DomainPosition>>> PostAsync(DomainPosition position) =>
        () => persistenceStore.CreateAsync(position);
}
