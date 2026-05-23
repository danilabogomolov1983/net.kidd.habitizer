namespace Net.Kidd.Habitizer.Tradix.Snapshot.Position;

public class PositionDbo
{
    public string? Account { get; set; } // Depot
    
    public string? Isin { get; set; } // ISIN
    
    public decimal? NetSize { get; set; } // Nominale

    public decimal? NetValue { get; set; } // Betrag
    
    public decimal? Price { get; set; } // SchnittKurs
    
    public string? Exchange { get; set; } // Boerse
    
    public decimal? UnrealisedProfit { get; set; } // UnrealPL
}
