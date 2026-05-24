namespace Net.Kidd.Habitizer.Features.Position.Bulk.DeleteByAccount;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
