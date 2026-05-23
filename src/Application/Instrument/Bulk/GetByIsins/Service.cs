using LanguageExt;
using Net.Kidd.Habitizer.Domain.Instrument;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Application.Instrument.Bulk.GetByIsins;

public class Service(IBulkStore bulkStore)
{
    public async Task<Fin<IReadOnlyList<Domain.Instrument.Instrument>>> GetByIsinsAsync(Command command)
    {
        var isins = command.Isins;
        var existingInstruments = await bulkStore.GetByIsinsAsync(command.Isins);

        var notFound = isins
            .Except(existingInstruments.Select(i => i.Isin))
            .ToList();
        return notFound.Count > 0
            ? Fin<IReadOnlyList<Domain.Instrument.Instrument>>.Fail("Some queried instruments were not found.")
            : existingInstruments.ToFin();
    }
}
