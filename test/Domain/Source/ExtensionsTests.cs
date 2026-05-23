using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Domain.Test.Source;

public class ExtensionsTests
{
    [Fact]
    public void WithName()
    {
        var source = NewSource();
        var name = new SourceName(NewFaker().Lorem.Word());

        var actual = source.WithName(name);

        Assert.Equal(name, actual.Name);
        Assert.NotSame(source, actual);
    }
}
