using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Account;

public sealed record Account(
    AccountId Id,
    Domain.Source.Source Source,
    AccountName Name,
    DateTimeOffset? LastUpdatedAt = null)
{
    public static Account Empty() => new(AccountId.Empty(), Domain.Source.Source.NotSpecified(), "NOT_SPECIFIED");
};
