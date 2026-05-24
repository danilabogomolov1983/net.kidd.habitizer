namespace Net.Kidd.Habitizer.Features.Position.Bulk.DeleteBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
