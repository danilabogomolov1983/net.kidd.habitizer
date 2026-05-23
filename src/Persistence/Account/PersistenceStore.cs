using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Domain.ValueObjects;


namespace Net.Kidd.Habitizer.Persistence.Account;

public class PersistenceStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Account.IPersistenceStore
{
    public Task<Fin<Domain.Account.Account>> CreateAsync(Domain.Account.Account account)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound =
                await context.Accounts
                    .Include(i => i.Source)
                    .FirstOrDefaultAsync(i => 
                    (i.Source.Name == account.Source.Name.Value && i.Name == account.Name.Value)
                    || i.Id == account.Id.Value);
            if (maybeFound != null)
            {
                return PersistenceErrors.AlreadyExists(account);
            }

            var mapped = DboMappers.AccountMapper.Map(account);
            var sourceAlreadyExists = await context.Sources
                .AnyAsync(i => i.Name == mapped.Source.Name);
            if (sourceAlreadyExists)
            {
                context.Attach(mapped.Source).State = EntityState.Unchanged;
            }

            await context.Accounts.AddAsync(mapped);
            await context.SaveChangesAsync();
            return Fin<Domain.Account.Account>.Succ(account);
        });

    public Task<Fin<Domain.Account.Account>> UpdateAsync(Domain.Account.Account account)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound = await context.Accounts
                .Include(i => i.Source)
                .FirstOrDefaultAsync(i => i.Id == account.Id.Value);

            if (maybeFound == null)
            {
                return PersistenceErrors.NotFound(account);
            }

            var mapped = DboMappers.AccountMapper.Map(account);
            context.Entry(maybeFound).CurrentValues.SetValues(mapped);

            await context.SaveChangesAsync();
            return Fin<Domain.Account.Account>.Succ(account);
        });

    public Task<Option<Domain.Account.Account>> GetByIdAsync(Domain.Account.AccountId accountId)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Accounts
                .Include(i => i.Source)
                .FirstOrDefaultAsync(i => i.Id == accountId.Value);
            return result == null
                ? Option<Domain.Account.Account>.None
                : DboMappers.AccountMapper.Map(result);
        });

    public Task<Option<Domain.Account.Account>> GetByNameAsync(AccountName name, SourceId sourceId)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Accounts
                .Include(i => i.Source)
                .FirstOrDefaultAsync(i => i.Name == name.Value && i.SourceId == sourceId.Value);
            return result == null
                ? Option<Domain.Account.Account>.None
                : DboMappers.AccountMapper.Map(result);
        });

    public Task<IReadOnlyList<Domain.Account.Account>> GetBySourceAsync(SourceId sourceId, int pageNumber, int pageSize)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Account.Account>>(dbContextFactory, async context =>
        {
            var page = Math.Max(pageNumber, 1);
            var size = Math.Max(pageSize, 10);

            var result = await context.Accounts
                .Where(i => i.SourceId == sourceId.Value)
                .Include(i => i.Source)
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();

            return result.Map(DboMappers.AccountMapper.Map).ToList();
        });
    public Task<IReadOnlyList<Domain.Account.Account>> GetListAsync(int pageNumber, int pageSize)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Account.Account>>(dbContextFactory, async context =>
        {
            var page = Math.Max(pageNumber, 1);
            var size = Math.Max(pageSize, 10);

            var result = await context.Accounts
                .Include(i => i.Source)
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();

            return result.Map(DboMappers.AccountMapper.Map).ToList();
        });
}
