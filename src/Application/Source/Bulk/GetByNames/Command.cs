using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
