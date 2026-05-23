namespace Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
