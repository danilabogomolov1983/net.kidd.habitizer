using Microsoft.Data.Sqlite;
using Net.Kidd.Habitizer.Persistence;

namespace Net.Kidd.Habitizer.TestCompanion;

public sealed class InMemoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public IPortfolioDbContextFactory ContextFactory { get; }

    public InMemoryFixture()
    {
        _connection = Support.Infrastructure.OpenConnection();
        ContextFactory = Support.Infrastructure.DbContextFactory(_connection);

        using var context = ContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        Support.Infrastructure.EnsureSchemaAsync(context).GetAwaiter().GetResult();
    }

    public void Dispose() => _connection.Dispose();
}

