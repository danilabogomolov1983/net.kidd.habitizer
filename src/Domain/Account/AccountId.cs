namespace Net.Kidd.Habitizer.Domain.Account;

public record AccountId(Guid Value)
{
    public static AccountId New() => new(Guid.NewGuid());
    public static AccountId Empty() => new(Guid.Empty);
    public static AccountId New(Guid AccountId) => new(AccountId);
}

