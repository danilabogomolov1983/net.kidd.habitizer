using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Persistence;
using Net.Kidd.Habitizer.Persistence.DbContext;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.TestCompanion;

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
