using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Source;

public sealed record Source(SourceId Id, SourceName Name)
{
    public static Source NotSpecified() => new Source(SourceId.Empty(), "NOT_SPECIFIED");
};
