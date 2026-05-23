using LanguageExt;
using Microsoft.EntityFrameworkCore;


namespace Net.Kidd.Habitizer.Persistence.Source;

public class PersistenceStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Source.IPersistenceStore
{
    public Task<Fin<Domain.Source.Source>> CreateAsync(Domain.Source.Source source)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var maybeFound =
                await context.Sources.FirstOrDefaultAsync(i => i.Name == source.Name.Value);
            if (maybeFound != null)
            {
                return PersistenceErrors.AlreadyExists(source);
            }

            var mapped = DboMappers.SourceMapper.Map(source);
            await context.Sources.AddAsync(mapped);
            await context.SaveChangesAsync();
            return Fin<Domain.Source.Source>.Succ(source);
        });


    public Task<Option<Domain.Source.Source>> GetByIdAsync(Domain.Source.SourceId sourceId)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Sources
                .FirstOrDefaultAsync(i => i.Id == sourceId.Value);
            return result == null
                ? Option<Domain.Source.Source>.None
                : DboMappers.SourceMapper.Map(result);
        });

    public Task<Option<Domain.Source.Source>> GetByNameAsync(Domain.ValueObjects.SourceName name)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var result = await context.Sources
                .FirstOrDefaultAsync(i => i.Name == name.Value);
            return result == null
                ? Option<Domain.Source.Source>.None
                : DboMappers.SourceMapper.Map(result);
        });

    public Task<IReadOnlyList<Domain.Source.Source>> GetListAsync(int pageNumber, int pageSize)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Source.Source>>(dbContextFactory, async context =>
        {
            var page = Math.Max(pageNumber, 1);
            var size = Math.Max(pageSize, 10);

            var result = await context.Sources
                .OrderByDescending(i => i.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();

            return result.Map(DboMappers.SourceMapper.Map).ToList();
        });
}
