namespace Wst.Tools.PosiBridge.Domain.Position;

public record PositionId(Guid Value)
{
    public static PositionId New() => new(Guid.NewGuid());
    public static PositionId Empty() => new(Guid.Empty);
    public static PositionId New(Guid PositionId) => new(PositionId);
}

