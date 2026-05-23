namespace Wst.Tools.PosiBridge.Domain.ValueObjects;

public record Isin(string Value)
{
    public static Isin Empty() => new(string.Empty);
    public static implicit operator Isin(string value) => new(value);

    public static implicit operator string(Isin isin) => isin.Value;

    public override string ToString() => Value;
    
    
};
