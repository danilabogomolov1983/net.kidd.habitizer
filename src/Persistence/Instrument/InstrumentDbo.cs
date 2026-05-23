namespace Net.Kidd.Habitizer.Persistence.Instrument;

public class InstrumentDbo
{
    public Guid Id { get; init; }
    public required string Isin { get; init; }
}
