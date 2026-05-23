namespace Net.Kidd.Habitizer.TickTs.IntegrationTest;

public partial class Support
{
    public static class TickTs
    {
        
        public static PosiBridge.TickTs.Snapshot.ReferencePrice.Response ReferencePrice()
        {
            var faker = NewFaker();
            return new PosiBridge.TickTs.Snapshot.ReferencePrice.Response(
                NewDecimal(),
                faker.PickRandom("EUR", "USD", "CHF"),
                faker.PickRandom("XETRA", "NYSE", "SIX"),
                NewDecimal()
            );
        }

        public static PosiBridge.TickTs.Snapshot.Position.Response Position(string? accountId = null)
        {
            var faker = NewFaker();
            return new PosiBridge.TickTs.Snapshot.Position.Response(
                accountId ?? faker.Random.Replace("???_####"),
                faker.Random.Replace("??##########"),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal());
        }
        
        public static PosiBridge.TickTs.Snapshot.Response SnapshotResponse(string? accountId = null) =>
            new([
                Position(accountId) with { ReferencePrice = ReferencePrice() },
                Position(accountId),
                Position(accountId)
            ]);

    }
    
}
