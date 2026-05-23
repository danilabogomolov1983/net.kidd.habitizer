using Wst.Tools.PosiBridge.Tradix.Snapshot.Position;

namespace Wst.Tools.PosiBridge.Tradix.Test;

public static partial class Support
{
    public static class Tradix
    {
        public const string Depot1 = "DEPOT1";
        public const string Depot2 = "DEPOT2";
        
        public static PositionDbo[] SeedPositions(int quantity, string account)
        {
            var faker = NewFaker();

            return Enumerable.Range(1, Math.Max(0, quantity))
                .Select(index =>
                {
                    var size = faker.Random.Int(1, 10000);
                    var averagePrice = decimal.Round(faker.Random.Decimal(0.01m, 10000m), 7);
                    var marketPrice = decimal.Round(faker.Random.Decimal(0.01m, 10000m), 2);
                    var exchange = faker.PickRandom("XETRA", "XFRA", "XHAM");
                    return new PositionDbo
                    {
                        Account = account,
                        Exchange = exchange,
                        Isin = faker.Random.Replace("??##########"),
                        NetSize = size,
                        NetValue = marketPrice * size,
                        Price = averagePrice,
                        UnrealisedProfit = decimal.Round(faker.Random.Decimal(-1000m, 1000m), 2)
                    };
                })
                .ToArray();
        }

        public static PositionDbo[] SeedInvalidPositions(int quantity, string account) =>
            Enumerable.Range(1, Math.Max(0, quantity))
                .Select(index => new PositionDbo
                {
                    Account = account,
                    Exchange = null,
                    Isin = null,
                    NetSize = null,
                    NetValue = null,
                    Price = null,
                    UnrealisedProfit = null
                })
                .ToArray();
    
        public static PositionDbo[] SeedNullPositions(int quantity, string account) =>
            Enumerable.Range(1, Math.Max(0, quantity))
                .Select(index => new PositionDbo
                {
                    Account = account,
                    Exchange = null,
                    Isin = null,
                    NetSize = null,
                    NetValue = null,
                    Price = null,
                    UnrealisedProfit = null
                })
                .ToArray();
    }
}
