namespace Net.Kidd.Habitizer.Application.Position.Bulk.DeleteBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
