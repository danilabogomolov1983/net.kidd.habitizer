using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.Persistence.Instrument;

namespace Wst.Tools.PosiBridge.Persistence.Position;

public class PositionDbo
{
    public Guid AccountId { get; set; }
    public Guid InstrumentId { get; set; }
    public decimal? NetSize { get; set; }
    public decimal? NetValue { get; set; }
    public decimal? UnrealisedAverageCost { get; set; }
    public decimal? UnrealisedProfit { get; set; }
    public decimal? UnrealisedProfitPercent { get; set; }
    public decimal? ReferencePrice_Price { get; set; }
    public string? ReferencePrice_Currency { get; set; }
    public string? ReferencePrice_Exchange { get; set; }
    public decimal? ReferencePrice_CurrencySpot { get; set; }
    
    
    public required AccountDbo Account { get; set; }
    public required InstrumentDbo Instrument { get; set; }
    
}
