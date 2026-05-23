namespace Wst.Tools.PosiBridge.Domain.ValueObjects;

public sealed record SourceName(string Value)
{
    public static implicit operator SourceName(string value) => new(value);

    public static implicit operator string(SourceName sourceName) => sourceName.Value;

    public override string ToString() => Value;
}
