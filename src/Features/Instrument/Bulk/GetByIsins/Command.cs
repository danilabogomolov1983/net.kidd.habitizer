using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Instrument.Bulk.GetByIsins;

public sealed record Command(IReadOnlyList<Isin> Isins);
