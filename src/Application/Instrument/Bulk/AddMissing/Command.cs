namespace Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Instrument.Instrument> Instruments);
