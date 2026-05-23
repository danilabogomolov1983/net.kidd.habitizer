using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Test.Source;

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
