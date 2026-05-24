using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
