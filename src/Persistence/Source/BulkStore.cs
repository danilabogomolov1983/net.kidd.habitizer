using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Domain.ValueObjects;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Persistence.Source;

public class BulkStore(IPortfolioDbContextFactory dbContextFactory) : Domain.Source.IBulkStore
{
    public Task<Fin<Unit>> InsertAsync(IReadOnlyList<Domain.Source.Source> sources)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var mapped = sources
                .Map(DboMappers.SourceMapper.Map)
                .ToImmutableList();

            await context.Sources.AddRangeAsync(mapped);
            await context.SaveChangesAsync();

            return Fin<Unit>.Succ(Unit.Default);
        });

    public Task<IReadOnlyList<Domain.Source.Source>> GetByNamesAsync(IReadOnlyList<SourceName> sourceNames)
        => DbContextFunctions.UsingContext<IReadOnlyList<Domain.Source.Source>>(dbContextFactory, async context =>
        {
            var isinValues = sourceNames.Select(i => i.Value)
                .ToImmutableList();

            var sources = await context.Sources
                .Where(i => isinValues.Contains(i.Name))
                .ToListAsync();

            return sources.Map(DboMappers.SourceMapper.Map).ToList();
        });

    public Task<Fin<Unit>> DeleteByNamesAsync(IReadOnlyList<SourceName> sourceNames)
        => DbContextFunctions.UsingContext(dbContextFactory, async context =>
        {
            var names = sourceNames.Select(i => i.Value)
                .ToImmutableList();

            await context.Sources
                .Where(i => names.Contains(i.Name))
                .ExecuteDeleteAsync();

            return Unit.Success();
        });
}
