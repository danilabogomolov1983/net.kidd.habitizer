using LanguageExt;
using Microsoft.EntityFrameworkCore;


namespace Wst.Tools.PosiBridge.Persistence.Position;

public class PersistenceStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Position.IPersistenceStore
{
    public Task<Fin<Domain.Position.Position>> CreateAsync(Domain.Position.Position position)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound =
                await context.Positions.FirstOrDefaultAsync(i =>
                    i.AccountId == position.Account.Id.Value &&
                    i.InstrumentId == position.Instrument.Id.Value);
            if (maybeFound != null)
            {
                return PersistenceErrors.AlreadyExists(position);
            }

            var mapped = DboMappers.PositionMapper.Map(position);

            // var sourceAlreadyExists = await context.Sources
            //     .AnyAsync(i => i.Name == mapped.Account.Source.Name);
            // if (sourceAlreadyExists)
            // {
            //     context.Attach(mapped.Account.Source).State = EntityState.Unchanged;
            // }

            var accountAlreadyExists = await context.Accounts
                .AnyAsync(i => i.Name == mapped.Account.Name && i.SourceId == mapped.Account.SourceId);
            if (accountAlreadyExists)
            {
                context.Attach(mapped.Account).State = EntityState.Unchanged;
            }

            var instrumentAlreadyExists = await context.Instruments
                .AnyAsync(i => i.Id == mapped.Instrument.Id);
            if (instrumentAlreadyExists)
            {
                context.Attach(mapped.Instrument).State = EntityState.Unchanged;
            }
            
            await context.Positions.AddAsync(mapped);
            await context.SaveChangesAsync();
            return Fin<Domain.Position.Position>.Succ(position);
        });


    public Task<Fin<Domain.Position.Position>> UpdateAsync(Domain.Position.Position position)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound = await context.Positions
                .Include(i => i.Account)
                    .ThenInclude(i => i.Source)
                .Include(i => i.Instrument)
                .FirstOrDefaultAsync(i =>
                    i.AccountId == position.Account.Id.Value &&
                    i.InstrumentId == position.Instrument.Id.Value);

            if (maybeFound == null)
            {
                return PersistenceErrors.NotFound(position);
            }

            var mapped = DboMappers.PositionMapper.Map(position);

            context.Entry(maybeFound).CurrentValues.SetValues(mapped);

            await context.SaveChangesAsync();
            return Fin<Domain.Position.Position>.Succ(position);
        });

    public Task<Option<Domain.Position.Position>> GetByIdAsync(Domain.Account.AccountId accountId, Domain.Instrument.InstrumentId instrumentId)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Positions
                .Include(i => i.Account)
                    .ThenInclude(i => i.Source)
                .Include(i => i.Instrument)
                .FirstOrDefaultAsync(i => i.AccountId == accountId.Value && i.InstrumentId == instrumentId.Value);
            return result == null
                ? Option<Domain.Position.Position>.None
                : DboMappers.PositionMapper.Map(result);
        });

    public Task<IReadOnlyList<Domain.Position.Position>> GetListAsync(int pageNumber, int pageSize)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Position.Position>>(dbContextFactory, async context =>
        {
            var page = Math.Max(pageNumber, 1);
            var size = Math.Max(pageSize, 10);

            var result = await context.Positions
                .Include(i => i.Account)
                    .ThenInclude(i => i.Source)
                .Include(i => i.Instrument)
                .OrderByDescending(i => i.AccountId)
                .ThenByDescending(i => i.InstrumentId)
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();

            return result.Map(DboMappers.PositionMapper.Map).ToList();
        });
}
