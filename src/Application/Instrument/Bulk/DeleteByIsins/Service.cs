using LanguageExt;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Instrument.Bulk.DeleteByIsins;

public class Service(GetByIsins.Service getByIsinsService, IBulkStore bulkStore)
{
    public Task<Fin<Unit>> DeleteAsync(Command command) =>
            getByIsinsService.GetByIsinsAsync(new GetByIsins.Command(command.Isins))
            .BindAsync(i =>
            {
                var isins = i.Select(k => k.Isin).ToList();
                return bulkStore.DeleteByIsinsAsync(isins);
            });
}
