
namespace Wst.Tools.PosiBridge.Application.Account.Bulk.Delete;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
