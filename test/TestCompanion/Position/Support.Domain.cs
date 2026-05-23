using static Wst.Tools.PosiBridge.TestCompanion.Support;

namespace Wst.Tools.PosiBridge.TestCompanion.Position;

public static class Support
{
    public static class Domain
    {
        public static decimal NewDecimal(int scale = 2) => Math.Round(NewFaker().Random.Decimal(1, 1000), scale);
        public static Wst.Tools.PosiBridge.Domain.Position.Position NewPosition()
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
