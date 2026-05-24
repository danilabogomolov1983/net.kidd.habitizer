namespace Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;

public sealed record Command(IReadOnlyList<Domain.Account.AccountId> AccountIds);
