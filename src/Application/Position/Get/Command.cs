using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Position.Get;

public sealed record Command(SourceName SourceName, AccountName AccountName, Isin Isin);

