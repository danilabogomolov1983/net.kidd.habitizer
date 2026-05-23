namespace Wst.Tools.PosiBridge.TestCompanion.Position;

public static class Support
{
    public static class Domain
    {
        public static Wst.Tools.PosiBridge.Domain.Position.Position NewPosition() =>
            new(
                Account.Support.Domain.NewAccount(),
                Instrument.Support.Domain.NewInstrument(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                new Wst.Tools.PosiBridge.Domain.Position.ReferencePrice.ReferencePrice(
                    NewDecimal(),
                    Wst.Tools.PosiBridge.TestCompanion.Support.NewFaker().PickRandom("EUR", "USD", "CHF"),
                    Wst.Tools.PosiBridge.TestCompanion.Support.NewFaker().PickRandom("XETRA", "NYSE", "SIX"),
                    NewDecimal()));
    }
}
