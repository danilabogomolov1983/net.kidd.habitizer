using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySourceAndNames;

public sealed record Command(SourceName SourceName, IReadOnlyList<AccountName> AccountNames);
