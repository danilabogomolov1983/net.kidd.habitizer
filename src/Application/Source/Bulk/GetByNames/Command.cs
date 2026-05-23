using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Application.Source.Bulk.GetByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
