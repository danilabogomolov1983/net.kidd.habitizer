namespace Net.Kidd.Habitizer.Features.Position.Bulk.GetBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
