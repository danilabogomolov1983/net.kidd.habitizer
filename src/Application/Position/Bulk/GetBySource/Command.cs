namespace Net.Kidd.Habitizer.Application.Position.Bulk.GetBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
