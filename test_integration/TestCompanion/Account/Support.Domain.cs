using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.TestCompanion.Account;

public static class Support
{
    public static class Domain
    {
        public static AccountName NewAccountName()
        {
            var faker = Net.Kidd.Habitizer.TestCompanion.Support.NewFaker();
            return new AccountName(faker.Random.Replace("?????_######") + faker.Random.Uuid());
        }

        public static Net.Kidd.Habitizer.Domain.Account.Account NewAccount() =>
            new(AccountId.New(), Source.Support.Domain.NewSource(), NewAccountName());
    }
}
