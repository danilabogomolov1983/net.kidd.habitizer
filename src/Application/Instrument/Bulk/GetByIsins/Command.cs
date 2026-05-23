using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Instrument.Bulk.GetByIsins;

public sealed record Command(IReadOnlyList<Isin> Isins);
