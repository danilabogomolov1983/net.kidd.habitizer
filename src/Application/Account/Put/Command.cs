using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Account.Put;

public sealed record Command(
    SourceName SourceName,
    AccountName AccountName,
    DateTimeOffset? LastUpdatedAt = null);
