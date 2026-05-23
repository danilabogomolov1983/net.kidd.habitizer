namespace Net.Kidd.Habitizer.TestCompanion.Position;

public static class Support
{
    public static class Domain
    {
        public static Net.Kidd.Habitizer.Domain.Position.Position NewPosition() =>
            new(
                Account.Support.Domain.NewAccount(),
                Instrument.Support.Domain.NewInstrument(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                new Net.Kidd.Habitizer.Domain.Position.ReferencePrice.ReferencePrice(
                    NewDecimal(),
                    Net.Kidd.Habitizer.TestCompanion.Support.NewFaker().PickRandom("EUR", "USD", "CHF"),
                    Net.Kidd.Habitizer.TestCompanion.Support.NewFaker().PickRandom("XETRA", "NYSE", "SIX"),
                    NewDecimal()));
    }
}
