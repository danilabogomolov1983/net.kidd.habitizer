using static Net.Kidd.Habitizer.TestCompanion.Support;

namespace Net.Kidd.Habitizer.TestCompanion.Position;

public static class Support
{
    public static class Domain
    {
        public static decimal NewDecimal(int scale = 2) => Math.Round(NewFaker().Random.Decimal(1, 1000), scale);
        public static Net.Kidd.Habitizer.Domain.Position.Position NewPosition()
            => new(Account.Support.Domain.NewAccount(),
                Instrument.Support.Domain.NewInstrument(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                NewDecimal(),
                ReferencePrice.Support.Domain.NewReferencePrice());
    }
}
