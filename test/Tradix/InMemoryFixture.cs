using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Tradix.Snapshot;

namespace Net.Kidd.Habitizer.Tradix.Test;

public sealed class InMemoryFixture
{
    public required IDbContextFactory<SnapshotDbContext> ContextFactory { get; init; }

    public InMemoryFixture()
    {
        var inMemoryContextFactory = Support.Infrastructure.DbContextFactory();
        var context = inMemoryContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();

        Support.Infrastructure.EnsureSchemaAsync(context).GetAwaiter().GetResult();
        ContextFactory = inMemoryContextFactory;
    }
}
