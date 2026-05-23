namespace Net.Kidd.Habitizer.Tradix.Configuration;

public class TradixSettings
{
    public const string ConfigurationSection = "TradixSettings";

    public string ConnectionString { get; init; } = string.Empty;
    
}


