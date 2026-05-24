namespace Net.Kidd.Habitizer.Features.Position.Bulk.GetByAccounts;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
