using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Application.Instrument.Bulk.GetByIsins;

public sealed record Command(IReadOnlyList<Isin> Isins);
