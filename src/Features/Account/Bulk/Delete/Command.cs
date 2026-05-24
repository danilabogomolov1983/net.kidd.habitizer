
namespace Net.Kidd.Habitizer.Features.Account.Bulk.Delete;

public sealed record Command(IReadOnlyList<Domain.Account.Account> Accounts);
