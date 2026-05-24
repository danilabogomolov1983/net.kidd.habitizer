namespace Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
