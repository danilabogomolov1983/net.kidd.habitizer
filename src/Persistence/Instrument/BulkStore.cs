using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Persistence.Instrument;

public class BulkStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Instrument.IBulkStore
{
    public Task<Fin<Unit>> InsertAsync(IReadOnlyList<Domain.Instrument.Instrument> instruments)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var mapped = instruments
                .Map(DboMappers.InstrumentMapper.Map)
                .ToImmutableList();

            await context.Instruments.AddRangeAsync(mapped);
            await context.SaveChangesAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<IReadOnlyList<Domain.Instrument.Instrument>> GetByIsinsAsync(IReadOnlyList<Isin> isins)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Instrument.Instrument>>(dbContextFactory, async context =>
        {
            var isinValues = isins.Select(i => i.Value)
                .ToImmutableList();

            var instruments = await context.Instruments
                .Where(i => isinValues.Contains(i.Isin))
                .AsNoTracking()
                .ToListAsync();

            return instruments.Map(DboMappers.InstrumentMapper.Map).ToList();
        });

    public Task<Fin<Unit>> DeleteByIsinsAsync(IReadOnlyList<Isin> isins)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var isinValues = isins.Select(i => i.Value)
                .ToImmutableList();
            var instrumentIds = await context.Instruments
                .Where(i => isinValues.Contains(i.Isin))
                .Select(i => i.Id)
                .ToListAsync();

            var hasReferences = await context.Positions
                .AnyAsync(i => instrumentIds.Contains(i.InstrumentId));
            if (hasReferences)
            {
                return Error.New("Cannot delete instruments referenced by positions.");
            }

            await context.Instruments
                .Where(i => isinValues.Contains(i.Isin))
                .ExecuteDeleteAsync();

            return Unit.Success();
        });
}
