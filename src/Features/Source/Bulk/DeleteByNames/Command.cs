using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Source.Bulk.DeleteByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
