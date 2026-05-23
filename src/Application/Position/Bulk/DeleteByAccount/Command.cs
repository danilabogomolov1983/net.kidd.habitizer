namespace Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteByAccount;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
