using Wst.Tools.PosiBridge.Domain.Account;

namespace Wst.Tools.PosiBridge.Domain.Test.Account;

public class ExtensionsTests
{
    [Fact]
    public void WithSourceId()
    {
        var account = NewAccount();
        var source = NewSource();

        var actual = account.WithSource(source);

        Assert.NotSame(account, actual);
        Assert.Equal(source, actual.Source);
    }

    [Fact]
    public void WithName()
    {
        var account = NewAccount();
        var name = NewAccountName();

        var actual = account.WithName(name);

        Assert.Equal(name, actual.Name);
        Assert.NotSame(account, actual);
    }
}
