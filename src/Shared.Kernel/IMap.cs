using System.Diagnostics.Contracts;

namespace Net.Kidd.Habitizer.Shared.Kernel;

public interface IMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight right);

    [Pure]
    TRight Map(TLeft left);

}

