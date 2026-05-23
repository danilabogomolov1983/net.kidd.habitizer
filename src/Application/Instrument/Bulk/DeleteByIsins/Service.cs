using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Instrument;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Instrument.Bulk.DeleteByIsins;

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
