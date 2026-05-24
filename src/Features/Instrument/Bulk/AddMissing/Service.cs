using LanguageExt;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<Unit>> AddMissingAsync(Command command)
    {
        var isins = command.Instruments
            .Select(i => i.Isin)
            .ToList();

        var existingInstruments = await bulkStore.GetByIsinsAsync(isins);
        var missingInstruments = command.Instruments
            .ExceptBy(existingInstruments.Select(i => i.Isin), i => i.Isin)
            .ToList();

        if (missingInstruments.Count == 0)
        {
            return await Unit.SuccessAsync();
        }

        return await bulkStore.InsertAsync(missingInstruments);
    }
}
