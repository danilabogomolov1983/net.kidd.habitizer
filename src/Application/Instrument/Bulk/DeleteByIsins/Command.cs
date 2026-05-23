using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Application.Instrument.Bulk.DeleteByIsins;

public sealed record Command(IReadOnlyList<Isin> Isins);
