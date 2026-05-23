namespace Net.Kidd.Habitizer.Domain.ValueObjects;

public sealed record AccountName(string Value)
{
    public static implicit operator AccountName(string value) => new(value);

    public static implicit operator string(AccountName accountName) => accountName.Value;

    public override string ToString() => Value;
}
