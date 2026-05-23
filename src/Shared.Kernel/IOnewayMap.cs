using System.Diagnostics.Contracts;

namespace Net.Kidd.Habitizer.Shared.Kernel;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight left);
}

