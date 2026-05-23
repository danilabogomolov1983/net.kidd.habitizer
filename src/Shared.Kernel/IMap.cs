using System.Diagnostics.Contracts;

namespace Wst.Tools.PosiBridge.Shared.Kernel;

public interface IMap<TLeft, TRight>
{
    [Pure]
    TLeft Map(TRight right);

    [Pure]
    TRight Map(TLeft left);

}

