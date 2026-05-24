namespace Net.Kidd.Habitizer.Features.Account.Bulk.Update;

public sealed record Command(Domain.ValueObjects.SourceName SourceName, IReadOnlyList<Domain.Account.Account> Accounts);
