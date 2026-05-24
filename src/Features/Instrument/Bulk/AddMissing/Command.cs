namespace Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Instrument.Instrument> Instruments);
