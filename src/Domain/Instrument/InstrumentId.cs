namespace Net.Kidd.Habitizer.Domain.Instrument;

public record InstrumentId(Guid Value)
{
    public static InstrumentId New() => new(Guid.NewGuid());
    public static InstrumentId Empty() => new(Guid.Empty);
    public static InstrumentId New(Guid InstrumentId) => new(InstrumentId);
}

