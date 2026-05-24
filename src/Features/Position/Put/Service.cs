using LanguageExt;
using AccountPost = Net.Kidd.Habitizer.Features.Account.Post;
using DomainPosition = Net.Kidd.Habitizer.Domain.Position.Position;
using InstrumentPost = Net.Kidd.Habitizer.Features.Instrument.Post;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;
using IPersistenceStore = Net.Kidd.Habitizer.Domain.Position.IPersistenceStore;

namespace Net.Kidd.Habitizer.Features.Position.Put;

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
