namespace Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
