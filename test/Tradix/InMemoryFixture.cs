using Microsoft.EntityFrameworkCore;
using Wst.Tools.PosiBridge.Tradix.Snapshot;

namespace Wst.Tools.PosiBridge.Tradix.Test;

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
