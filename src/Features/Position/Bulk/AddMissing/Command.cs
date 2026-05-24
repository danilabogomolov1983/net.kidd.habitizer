namespace Net.Kidd.Habitizer.Features.Position.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Position.Position> Positions);
