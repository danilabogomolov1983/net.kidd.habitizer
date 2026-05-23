using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Persistence.Position;

public class BulkStore(IPortfolioDbContextFactory dbContextFactory) : IBulkStore
{
    public Task<Fin<Unit>> InsertAsync(IReadOnlyList<Domain.Position.Position> positions)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var accounts = await context.Accounts
                .Include(i=>i.Source)
                .Where(i => positions
                    .Select(k => k.Account.Name.Value).Contains(i.Name))
                .ToListAsync();

            if (accounts.Count == 0)
            {
                return PersistenceErrors.NotFound("no accounts found");
            }
            
            var instruments = await context.Instruments
                .Where(i => positions.Select(k => k.Instrument.Isin.Value).Contains(i.Isin))
                .ToListAsync();

            if (instruments.Count == 0)
            {
                return PersistenceErrors.NotFound("no instruments found");
            }
            
            
            var mapped = positions
                .Map(DboMappers.PositionMapper.Map)
                .Map(i =>
                {
                    i.Account = accounts.First(k => k.Source.Name == i.Account.Source.Name && k.Name == i.Account.Name);
                    context.Attach(i.Account).State = EntityState.Unchanged;

                    i.Instrument = instruments.First(k => k.Isin == i.Instrument.Isin);
                    context.Attach(i.Instrument).State = EntityState.Unchanged;
                    return i;
                })
                .ToImmutableList();
            
            await context.Positions.AddRangeAsync(mapped);
            await context.SaveChangesAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<IReadOnlyList<Domain.Position.Position>> GetByAccountIdsAsync(IReadOnlyList<Domain.Account.AccountId> accountIds)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Position.Position>>(dbContextFactory, async context =>
        {
            var accountPositions = await context.Positions
                .Include(i => i.Instrument)
                .Include(i => i.Account)
                    .ThenInclude(i => i.Source)
                .AsNoTracking()
                .Where(i => accountIds.Select(k => k.Value).Contains(i.AccountId))
                .ToListAsync();

            return accountPositions.Map(DboMappers.PositionMapper.Map).ToList();
        });

    public Task<IReadOnlyList<Domain.Position.Position>> GetBySourceAsync(IReadOnlyList<Domain.Source.Source> sources)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Position.Position>>(dbContextFactory, async context =>
        {
            var sourceIds = sources.Select(i => i.Id.Value)
                .ToImmutableList();
            var accountPositions = await context.Positions
                .Include(i => i.Instrument)
                    .Include(i => i.Account)
                        .ThenInclude(i => i.Source)
                .AsNoTracking()
                .Where(i => sourceIds.Contains(i.Account.SourceId))
                .ToListAsync();

            return accountPositions.Map(DboMappers.PositionMapper.Map).ToList();
        });

    public Task<Fin<Unit>> DeleteBySourceAsync(IReadOnlyList<Domain.Source.Source> sources)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var sourceIds = sources.Select(i => i.Id.Value).ToList();
            await context.Positions
                .Include(i => i.Instrument)
                .Include(i => i.Account)
                .ThenInclude(i => i.Source)
                .AsNoTracking()
                .Where(i => sourceIds.Contains(i.Account.SourceId))
                .ExecuteDeleteAsync();

            return Unit.Success();
        });

    public Task<Fin<Unit>> DeleteByAccountAsync(IReadOnlyList<Domain.Account.Account> accounts)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var accountIds = accounts.Select(i => i.Id.Value)
                .ToImmutableList();

            await context.Positions
                .Include(i => i.Account)
                .Where(i => accountIds.Contains(i.AccountId))
                .ExecuteDeleteAsync();

            return Unit.Success();
        });
}
