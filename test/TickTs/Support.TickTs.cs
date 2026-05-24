namespace Net.Kidd.Habitizer.TickTs.Test;

public static partial class Support
{
    public static class TickTs
    {
        
        public static Habitizer.TickTs.Snapshot.ReferencePrice.Response ReferencePrice()
        {
            var faker = NewFaker();
            return new Habitizer.TickTs.Snapshot.ReferencePrice.Response(
                NewDecimal(),
                faker.PickRandom("EUR", "USD", "CHF"),
                faker.PickRandom("XETRA", "NYSE", "SIX"),
                NewDecimal()
            );
        }

        public static Habitizer.TickTs.Snapshot.Position.Response Position()
        {
            var faker = NewFaker();
            return new Habitizer.TickTs.Snapshot.Position.Response(
                faker.Random.Replace("???_####"),
                faker.Random.Replace("??##########"),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal());
        }
        
        public static Habitizer.TickTs.Snapshot.Response SnapshotResponse() =>
            new([
                Position() with { ReferencePrice = ReferencePrice() },
                Position(),
                Position()
            ]);

    }
}
