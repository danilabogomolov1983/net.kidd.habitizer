using System.Diagnostics.Contracts;

namespace Wst.Tools.PosiBridge.Shared.Kernel;

public interface IOnewayMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight left);
}

