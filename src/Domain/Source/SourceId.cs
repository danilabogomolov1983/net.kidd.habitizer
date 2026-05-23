namespace Wst.Tools.PosiBridge.Domain.Source;

public record SourceId(Guid Value)
{
    public static SourceId New() => new(Guid.NewGuid());
    public static SourceId Empty() => new(Guid.Empty);
    public static SourceId New(Guid SourceId) => new(SourceId);
}

