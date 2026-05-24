using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Account.Put;

public sealed record Command(
    SourceName SourceName,
    AccountName AccountName,
    DateTimeOffset? LastUpdatedAt = null);
