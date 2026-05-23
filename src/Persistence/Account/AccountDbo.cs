using Net.Kidd.Habitizer.Persistence.Source;
using Net.Kidd.Habitizer.Persistence.Position;

namespace Net.Kidd.Habitizer.Persistence.Account;

public class AccountDbo
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTimeOffset? LastUpdatedAt { get; set; }

    public Guid SourceId { get; init; }
    public required SourceDbo Source { get; set; }
    public ICollection<PositionDbo> Positions { get; set; } = new List<PositionDbo>();
}
