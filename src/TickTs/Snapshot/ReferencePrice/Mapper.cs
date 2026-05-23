using Wst.Tools.PosiBridge.Shared.Kernel;

namespace Wst.Tools.PosiBridge.TickTs.Snapshot.ReferencePrice;

public struct Mapper : IMap<Domain.Position.ReferencePrice.ReferencePrice, Response>
{
    public Domain.Position.ReferencePrice.ReferencePrice Map(Response right)
    {
        return new Domain.Position.ReferencePrice.ReferencePrice(
            right.Price,
            right.CurrencyId,
            right.ExchangeId,
            right.CurrencySpot);
    }

    public Response Map(Domain.Position.ReferencePrice.ReferencePrice left)
    {
        return new Response(
            left.Price,
            left.Currency,
            left.Exchange,
            left.CurrencySpot);
    }
}
