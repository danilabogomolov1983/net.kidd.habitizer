using static Net.Kidd.Habitizer.TestCompanion.Support;


namespace Net.Kidd.Habitizer.TestCompanion.Position.ReferencePrice;

public static class Support
{
    public static class Domain
    {
        
        public static string NewCurrency() => NewFaker().PickRandom("EUR", "USD", "CHF");
        public static string NewExchange() => NewFaker().PickRandom("XETRA", "NYSE", "SIX");

        public static Net.Kidd.Habitizer.Domain.Position.ReferencePrice.ReferencePrice NewReferencePrice()
            => new(NewDecimal(),
                    NewCurrency(),
                    NewExchange(),
                    NewDecimal());
    }
}
