using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Instrument;

public static class Extensions
{
    extension(Instrument instrument)
    {
        public Instrument WithIsin(Isin isin) =>
            instrument with { Isin = isin };
        
        public bool IsEmpty() => instrument == Instrument.Empty();
    }
}
