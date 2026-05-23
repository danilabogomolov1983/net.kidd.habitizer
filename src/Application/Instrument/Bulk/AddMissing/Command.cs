namespace Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Instrument.Instrument> Instruments);
