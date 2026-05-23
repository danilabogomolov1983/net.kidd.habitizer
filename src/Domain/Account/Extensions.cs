using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Account;

public static class Extensions
{
    extension(Account account)
    {
        public bool IsEmpty() => account == Account.Empty();
        
        public Account WithSource(Domain.Source.Source source) =>
            account with { Source = source };

        public Account WithName(string name) =>
            account with { Name = new AccountName(name) };

        public Account WithName(AccountName name) =>
            account with { Name = name };
        
        public Account WithLastUpdatedAt(DateTimeOffset? lastUpdatedAt) =>
            account with { LastUpdatedAt = lastUpdatedAt };
    }
}
