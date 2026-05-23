using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Tradix.Snapshot;
using Net.Kidd.Habitizer.Tradix.Snapshot.Position;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.Tradix.Test;

public static partial class Support
{
    public static class Infrastructure
    {
        public static IDbContextFactory<SnapshotDbContext> DbContextFactory()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            return SqliteOptions(connection)
                .Then(options => ContextFactory(() => new SnapshotDbContext(options)));
        }

        public static async Task EnsureSchemaAsync(SnapshotDbContext context)
        {

            await context.Database.ExecuteSqlRawAsync($"""
                                                       CREATE TABLE IF NOT EXISTS WELCOME_TRX_ShortPositionsOverview
                                                       (
                                                           Depot       varchar(50)    NULL,
                                                           ISIN        varchar(12)    NULL,
                                                           Nominale    numeric(15, 0) NULL,
                                                           Betrag      numeric(15, 2) NULL,
                                                           SchnittKurs numeric(15, 7) NULL,
                                                           Boerse      varchar(50)    NULL,
                                                           UnrealPL    numeric(15, 2) NULL
                                                       )
                                                       """,
                TestContext.Current.CancellationToken);
        }

        public static async Task InsertPositionAsync(SnapshotDbContext context, PositionDbo position)
        {
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                 INSERT INTO WELCOME_TRX_ShortPositionsOverview
                 (ISIN, Betrag, Nominale, Boerse, UnrealPL, Depot, SchnittKurs)
                 VALUES
                 ({position.Isin}, {position.NetValue}, {position.NetSize}, {position.Exchange}, {position.UnrealisedProfit}, {position.Account}, {position.Price});
                 """);
        }

        public static async Task DeletePositionsAsync(SnapshotDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM WELCOME_TRX_ShortPositionsOverview;
                """);
        }

        private static DbContextOptions<SnapshotDbContext> SqliteOptions(SqliteConnection connection) =>
            new DbContextOptionsBuilder<SnapshotDbContext>()
                .UseSqlite(connection)
                .Options;

        private static IDbContextFactory<SnapshotDbContext> ContextFactory(
            Func<SnapshotDbContext> contextFunc)
        {
            var contextFactory = A.Fake<IDbContextFactory<SnapshotDbContext>>();

            A.CallTo(() => contextFactory.CreateDbContextAsync(A<CancellationToken>._))
                .ReturnsLazily(contextFunc);
            return contextFactory;
        }
    }
}
