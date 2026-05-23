using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Wst.Tools.PosiBridge.Persistence;
using Wst.Tools.PosiBridge.Persistence.DbContext;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.TestCompanion;

public static partial class Support
{
    public static class Infrastructure
    {
        public static SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            return connection;
        }

        public static IPortfolioDbContextFactory DbContextFactory(SqliteConnection connection) => SqliteOptions(connection)
            .Then(o => ContextFactory(() => new PortfolioDbContext(o)));

        public static Task EnsureSchemaAsync(PortfolioDbContext context) =>
            context.Database.EnsureCreatedAsync();

        private static DbContextOptions<PortfolioDbContext> SqliteOptions(SqliteConnection connection) =>
            new DbContextOptionsBuilder<PortfolioDbContext>()
                .UseSqlite(connection).Options;

        private static IPortfolioDbContextFactory ContextFactory(
            Func<PortfolioDbContext> contextFunc) 
        {
            var contextFactory = A.Fake<IPortfolioDbContextFactory>();

            A.CallTo(() => contextFactory.CreateDbContextAsync(A<CancellationToken>._))
                .ReturnsLazily(contextFunc);
            return contextFactory;
        }
    }
}
