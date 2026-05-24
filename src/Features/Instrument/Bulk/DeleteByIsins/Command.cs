using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Instrument.Bulk.DeleteByIsins;

public sealed record Command(IReadOnlyList<Isin> Isins);
