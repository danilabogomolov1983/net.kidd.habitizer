using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Tradix.Snapshot;
using Net.Kidd.Habitizer.Tradix.Snapshot.Position;

namespace Net.Kidd.Habitizer.Tradix.IntegrationTest;

public static partial class Support
{
    public static class Infrastructure
    {
        public static async Task EnsureSchemaAsync(SnapshotDbContext context)
        {

            await context.Database.ExecuteSqlRawAsync("""
                                                        IF OBJECT_ID(N'[dbo].[WELCOME_TRX_ShortPositionsOverview]', N'U') IS NULL
                                                        BEGIN
                                                            CREATE TABLE [dbo].[WELCOME_TRX_ShortPositionsOverview]
                                                            (
                                                                Depot       varchar(50)    NULL,
                                                                ISIN        varchar(12)    NULL,
                                                                Nominale    numeric(15, 0) NULL,
                                                                Betrag      numeric(15, 2) NULL,
                                                                SchnittKurs numeric(15, 7) NULL,
                                                                Boerse      varchar(50)    NULL,
                                                                UnrealPL    numeric(15, 2) NULL
                                                            )
                                                        END
                                                        """,
                TestContext.Current.CancellationToken);
        }

        public static async Task InsertPositionAsync(SnapshotDbContext context, PositionDbo position)
        {
            // Parameters are typed explicitly so decimals are not silently coerced to the
            // EF Core default mapping of decimal(18, 2). The precision/scale below mirror
            // the column definitions in EnsureSchemaAsync.
            var parameters = new object[]
            {
                new SqlParameter("@isin", SqlDbType.VarChar, 12)
                {
                    Value = (object?)position.Isin ?? DBNull.Value
                },
                new SqlParameter("@betrag", SqlDbType.Decimal)
                {
                    Precision = 15,
                    Scale = 2,
                    Value = (object?)position.NetValue ?? DBNull.Value
                },
                new SqlParameter("@nominale", SqlDbType.Decimal)
                {
                    Precision = 15,
                    Scale = 0,
                    Value = (object?)position.NetSize ?? DBNull.Value
                },
                new SqlParameter("@boerse", SqlDbType.VarChar, 50)
                {
                    Value = (object?)position.Exchange ?? DBNull.Value
                },
                new SqlParameter("@unrealPL", SqlDbType.Decimal)
                {
                    Precision = 15,
                    Scale = 2,
                    Value = (object?)position.UnrealisedProfit ?? DBNull.Value
                },
                new SqlParameter("@depot", SqlDbType.VarChar, 50)
                {
                    Value = (object?)position.Account ?? DBNull.Value
                },
                new SqlParameter("@schnittKurs", SqlDbType.Decimal)
                {
                    Precision = 15,
                    Scale = 7,
                    Value = (object?)position.Price ?? DBNull.Value
                }
            };

            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO WELCOME_TRX_ShortPositionsOverview
                (ISIN, Betrag, Nominale, Boerse, UnrealPL, Depot, SchnittKurs)
                VALUES
                (@isin, @betrag, @nominale, @boerse, @unrealPL, @depot, @schnittKurs);
                """,
                parameters);
        }

        public static async Task DeletePositionsAsync(SnapshotDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM WELCOME_TRX_ShortPositionsOverview;
                """);
        }
    }
}
