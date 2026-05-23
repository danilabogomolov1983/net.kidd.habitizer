using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Persistence.Account;

public class BulkStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Account.IBulkStore
{
    public Task<Fin<Unit>> InsertAsync(IReadOnlyList<Domain.Account.Account> accounts)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var sources = await context.Sources
                .Where(i => accounts.Select(k => k.Source.Name.Value).Contains(i.Name))
                .ToListAsync();

            if (sources.Count == 0)
            {
                return PersistenceErrors.NotFound("no sources found");
            }
            var mapped = accounts
                .Map(DboMappers.AccountMapper.Map)
                .Map(i =>
                {
                    i.Source = sources.First(k => k.Name == i.Source.Name);
                    context.Attach(i.Source).State = EntityState.Unchanged;
                    return i;
                })
                .ToImmutableList();
            
            await context.Accounts.AddRangeAsync(mapped);
            await context.SaveChangesAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<Fin<Unit>> UpdateAsync(IReadOnlyList<Domain.Account.Account> accounts)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var accountIds = accounts.Select(i => i.Id.Value);
            var existingIds = await context.Accounts
                .Where(i => accountIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            var missingAccounts = accountIds.Except(existingIds);
            if (!missingAccounts.IsNullOrEmpty())
            {
                return PersistenceErrors.NotFound("Some accounts are not found");
            }

            var sourceNames = accounts.Select(i => i.Source.Name.Value)
                .Distinct()
                .ToImmutableList();

            var sources = await context.Sources
                .Where(i => sourceNames.Contains(i.Name))
                .ToListAsync();

            if (sources.Count != sourceNames.Count)
            {
                return PersistenceErrors.NotFound("no sources found");
            }
            
            var mapped = accounts
                .Map(DboMappers.AccountMapper.Map)
                .Map(i =>
                {
                    i.Source = sources.First(k => k.Name == i.Source.Name);
                    context.Attach(i.Source).State = EntityState.Unchanged;
                    return i;
                })
                .ToImmutableList();
            
            context.Accounts.UpdateRange(mapped);
            await context.SaveChangesAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<IReadOnlyList<Domain.Account.Account>> GetBySourcesAsync(IReadOnlyList<Domain.Source.Source> sources)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Account.Account>>(dbContextFactory, async context =>
        {
            var sourceIds = sources.Select(i => i.Id.Value)
                .ToImmutableList();

            var accounts = await context.Accounts
                .Include(i => i.Source)
                .AsNoTracking()
                .Where(i => sourceIds.Contains(i.SourceId))
                .ToListAsync();

            return accounts.Map(DboMappers.AccountMapper.Map).ToList();
        });

    public Task<IReadOnlyList<Domain.Account.Account>> GetBySourceAndNamesAsync(SourceName sourceName, IReadOnlyList<AccountName> accountNames)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Account.Account>>(dbContextFactory, async context =>
        {
            var names = accountNames.Select(i => i.Value)
                .ToImmutableList();

            var accounts = await context.Accounts
                .Include(i => i.Source)
                .AsNoTracking()
                .Where(i => i.Source.Name == sourceName.Value && names.Contains(i.Name))
                .ToListAsync();

            return accounts.Map(DboMappers.AccountMapper.Map).ToList();
        });

    public Task<IReadOnlyList<Domain.Account.Account>> GetByIdsAsync(IReadOnlyList<Domain.Account.AccountId> accountIds)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Account.Account>>(dbContextFactory, async context =>
        {
            var accounts = await context.Accounts
                .Include(i => i.Source)
                .AsNoTracking()
                .Where(i => accountIds.Select(i => i.Value).Contains(i.Id))
                .ToListAsync();

            return accounts.Map(DboMappers.AccountMapper.Map).ToList();
        });

    public Task<Fin<Unit>> DeleteAsync(IReadOnlyList<Domain.Account.Account> accounts)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var names = accounts
                .Select(i => i.Id.Value)
                .ToImmutableList();

            await context.Accounts
                .Where(i => names.Contains(i.Id))
                .ExecuteDeleteAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<Fin<Unit>> DeleteByIdsAsync(IReadOnlyList<Domain.Account.AccountId> accountIds)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            await context.Accounts
                .Where(i => accountIds.Select(i => i.Value).Contains(i.Id))
                .ExecuteDeleteAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<Fin<Unit>> DeleteBySourceAsync(IReadOnlyList<Domain.Source.Source> sources)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var sourceIds = sources.Select(i => i.Id.Value)
                .ToImmutableList();

            await context.Accounts
                .Include(i => i.Source)
                .Where(i => sourceIds.Contains(i.SourceId))
                .ExecuteDeleteAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

}
