namespace Wst.Tools.PosiBridge.Domain.Position;

public sealed record Position(
    Account.Account Account,
    Instrument.Instrument Instrument,
    decimal? NetSize = null,
    decimal? NetValue = null,
    decimal? UnrealisedAverageCost = null,
    decimal? UnrealisedProfit = null,
    decimal? UnrealisedProfitPercent = null,
    ReferencePrice.ReferencePrice? ReferencePrice = null);
