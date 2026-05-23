namespace Net.Kidd.Habitizer.TickTs.Configuration;

public class TickTsSettings
{
    public const string ConfigurationSection = "TickTsSettings";

    public string BaseUrl { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public string ResolvedAddress { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 30;
}


