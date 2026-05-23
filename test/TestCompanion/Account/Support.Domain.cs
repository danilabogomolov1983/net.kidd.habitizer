using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.TestCompanion.Account;

public static class Support
{
    public static class Domain
    {
        public static AccountName NewAccountName() => new(Net.Kidd.Habitizer.TestCompanion.Support.NewFaker().Random.Replace("???_####"));

        public static Net.Kidd.Habitizer.Domain.Account.Account NewAccount() =>
            new(AccountId.New(), Source.Support.Domain.NewSource(), NewAccountName());
    }
}
