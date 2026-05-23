using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Source.Bulk.DeleteByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
