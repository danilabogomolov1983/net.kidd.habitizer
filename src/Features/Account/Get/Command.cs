using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Account.Get;

public sealed record Command(SourceName SourceName, AccountName Name);

