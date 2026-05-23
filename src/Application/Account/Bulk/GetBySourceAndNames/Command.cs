using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Application.Account.Bulk.GetBySourceAndNames;

public sealed record Command(SourceName SourceName, IReadOnlyList<AccountName> AccountNames);
