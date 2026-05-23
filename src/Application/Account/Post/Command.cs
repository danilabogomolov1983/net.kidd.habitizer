using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Account.Post;

public sealed record Command(
    SourceName SourceName,
    AccountName AccountName,
    DateTimeOffset? LastUpdatedAt = null);
