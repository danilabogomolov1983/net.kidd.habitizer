namespace Wst.Tools.PosiBridge.TestCompanion.Position.ReferencePrice;

public static class Support
{
    public static class Domain
    {
        public static string NewCurrency() => NewFaker().PickRandom("EUR", "USD", "CHF");
        public static string NewExchange() => NewFaker().PickRandom("XETRA", "NYSE", "SIX");

        public static Wst.Tools.PosiBridge.Domain.Position.ReferencePrice.ReferencePrice NewReferencePrice()
            => new(
                NewDecimal(),
                NewCurrency(),
                NewExchange(),
                NewDecimal());
    }
}
