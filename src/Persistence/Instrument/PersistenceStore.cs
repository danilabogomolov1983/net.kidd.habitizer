using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Domain.ValueObjects;


namespace Net.Kidd.Habitizer.Persistence.Instrument;

public class PersistenceStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Instrument.IPersistenceStore
{
    public Task<Fin<Domain.Instrument.Instrument>> CreateAsync(Domain.Instrument.Instrument instrument)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound =
                await context.Instruments.FirstOrDefaultAsync(i =>
                    i.Isin == instrument.Isin.Value || i.Id == instrument.Id.Value);
            if (maybeFound != null)
            {
                return PersistenceErrors.AlreadyExists(instrument);
            }

            var mapped = DboMappers.InstrumentMapper.Map(instrument);
            await context.Instruments.AddAsync(mapped);
            await context.SaveChangesAsync();
            return Fin<Domain.Instrument.Instrument>.Succ(instrument);
        });


    public Task<Option<Domain.Instrument.Instrument>> GetByIdAsync(Domain.Instrument.InstrumentId instrumentId)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Instruments
                .FirstOrDefaultAsync(i => i.Id == instrumentId.Value);
            return result == null
                ? Option<Domain.Instrument.Instrument>.None
                : DboMappers.InstrumentMapper.Map(result);
        });

    public Task<Option<Domain.Instrument.Instrument>> GetByIsinAsync(Isin isin)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Instruments
                .FirstOrDefaultAsync(i => i.Isin == isin.Value);
            return result == null
                ? Option<Domain.Instrument.Instrument>.None
                : DboMappers.InstrumentMapper.Map(result);
        });

    public Task<IReadOnlyList<Domain.Instrument.Instrument>> GetListAsync(int pageNumber, int pageSize)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Instrument.Instrument>>(dbContextFactory,
            async context =>
            {
                var page = Math.Max(pageNumber, 1);
                var size = Math.Max(pageSize, 10);

                var result = await context.Instruments
                    .OrderByDescending(i => i.Id)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .AsNoTracking()
                    .ToListAsync();

                return result.Map(DboMappers.InstrumentMapper.Map).ToList();
            });
}
