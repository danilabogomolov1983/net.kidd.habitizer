using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Account;

public sealed record Account(
    AccountId Id,
    Domain.Source.Source Source,
    AccountName Name,
    DateTimeOffset? LastUpdatedAt = null)
{
    public static Account Empty() => new(AccountId.Empty(), Domain.Source.Source.NotSpecified(), "NOT_SPECIFIED");
};
