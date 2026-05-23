using LanguageExt;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<Unit>> AddMissingAsync(Command command)
    {
        var sourceNames = command.Sources
            .Select(source => source.Name)
            .ToList();

        var existingSources = await bulkStore.GetByNamesAsync(sourceNames);
        var missingSources = command.Sources
            .ExceptBy(existingSources.Select(source => source.Name), source => source.Name)
            .ToList();

        if (missingSources.Count == 0)
        {
            return await Unit.SuccessAsync();
        }

        return await bulkStore.InsertAsync(missingSources);
    }
}
