using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Account.Get;

public sealed record Command(SourceName SourceName, AccountName Name);

