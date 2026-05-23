namespace Net.Kidd.Habitizer.Application.Position.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Position.Position> Positions);
