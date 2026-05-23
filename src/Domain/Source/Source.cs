using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Source;

public sealed record Source(SourceId Id, SourceName Name)
{
    public static Source NotSpecified() => new Source(SourceId.Empty(), "NOT_SPECIFIED");
};
