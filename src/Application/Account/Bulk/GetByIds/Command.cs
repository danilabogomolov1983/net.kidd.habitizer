namespace Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;

public sealed record Command(IReadOnlyList<Domain.Account.AccountId> AccountIds);
