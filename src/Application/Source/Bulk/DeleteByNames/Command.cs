using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Application.Source.Bulk.DeleteByNames;

public sealed record Command(IReadOnlyList<SourceName> SourceNames);
