namespace Wst.Tools.PosiBridge.Application.Position.Bulk.GetByAccounts;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
