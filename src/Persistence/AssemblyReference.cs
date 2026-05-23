using System.Reflection;

namespace Net.Kidd.Habitizer.Persistence;

public record Anchor;
// ReSharper disable once UnusedType.Global
public static class AssemblyReference
{
    public static Assembly Assembly => typeof(Anchor).Assembly;
}

